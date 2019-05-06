using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceProtocol.ObjectManager;
using ServiceProtocol.Data.Entity;
using ServiceProtocol.Auth;
using ServiceProtocol.Logging;
using ServiceProtocol.Events;

namespace ServiceProtocol.Biz.Accounting.Entity
{
    [DataObjectLoader("账户充值记录")]
    public class AccountService<
		TAccountPublic, TAccountInternal, 
        TDepositRecordPublic,
        TTransferRecordPublic,
        TRefundRecordPublic,
        TAccount, TTitle, TDepositRecord,TTransferRecord,TRefundRecord
        > :
		IAccountService,
        IDataObjectLoader
        where TAccountPublic:Account,new()
        where TAccountInternal : AccountInternal, new()
        where TDepositRecordPublic:DepositRecord,new()
        where TTransferRecordPublic:TransferRecord,new()
        where TRefundRecordPublic:RefundRecord,new()

        where TAccount : Models.Account<TTitle>,new()
		where TTitle: Models.AccountTitle<TAccount>
		where TDepositRecord: Models.DepositRecord<TAccount,TTitle>,new()
        where TRefundRecord : Models.RefundRecord<TDepositRecord,TAccount, TTitle>, new()
        where TTransferRecord : Models.TransferRecord<TAccount, TTitle,TTransferRecord>,new()
	{	
		public IDataContext Context { get; }
		public Times.ITimeService TimeService { get; }
		public Lazy<ServiceProtocol.Biz.Payments.ICollectService> CollectService { get; }
        public Lazy<CallGuarantors.ICallGuarantor> CallGuarantor { get; }
        public Lazy<ServiceProtocol.ObjectManager.IDataObjectResolver> ObjectResolver { get; }
        public ILogger Logger { get; }
        public Lazy<ServiceProtocol.Events.IEventEmitter> EventEmitter { get; }
		public AccountService(
			IDataContext Context, 
			Times.ITimeService TimeService, 
			Lazy<CallGuarantors.ICallGuarantor> CallGuarantor,
			Lazy<ServiceProtocol.Biz.Payments.ICollectService> CollectService,
           Lazy<ServiceProtocol.ObjectManager.IDataObjectResolver> ObjectResolver,
            ILogService LogService,
            Lazy<ServiceProtocol.Events.IEventEmitter> EventEmitter
            )
		{
			this.Context = Context;
			this.TimeService = TimeService;
			this.CallGuarantor = CallGuarantor;
			this.CollectService = CollectService;
            this.ObjectResolver = ObjectResolver;
            this.Logger = LogService.GetLogger(GetType());
            this.EventEmitter = EventEmitter;
        }

        #region 充值
        public async Task<int> CreateDeposit(DepositArgument Arg)
        {
            var title = await Context.ReadOnly<TTitle>()
                   .Where(t => t.Ident == Arg.AccountTitle)
                   .Select(t => t.Id).SingleOrDefaultAsync();
            if (title == 0)
                throw new ArgumentException("账户科目不存在:" + Arg.AccountTitle);
            Arg.Amount = Math.Round(Arg.Amount);
            if (Arg.Amount <= 0 || Arg.Amount > 1000000)
                throw new ArgumentException("金额不合法");

            var time = TimeService.Now;
            var record = Context.Add(new TDepositRecord
            {
                AccountTitleId = title,
                DstId = Arg.DstId,
                OperatorId = Arg.OperatorId,
                Amount = Arg.Amount,
                Title = Arg.Description.Limit(200),
                TrackEntityIdent = Arg.TrackEntityIdent,
                CreatedTime = time,
                CallbackName = Arg.CallbackName,
                CallbackContext = Arg.CallbackContext,
                PaymentPlatformId = Arg.PaymentPlatformId,
                State = DepositState.New,
                OpAddress = Arg.OpAddress,
                OpDevice = Arg.OpDevice,
            });
            await Context.SaveChangesAsync();

            var ident = Ident.CreateDepositIdent(time, Arg.DstId.ToString(), record.Id);
            await CollectService.Value.Create(new Payments.CollectRequest
            {
                Amount = record.Amount,
                Desc = record.Title,
                Title = record.Title,
                Ident = ident,
                PaymentPlatformId = record.PaymentPlatformId,
                ClientType = Arg.ClientType,
                OpAddress = record.OpAddress,
                OpDevice = record.OpDevice,
                TrackEntityIdent = "账户充值记录-" + record.Id,
                CurUserId = record.DstId.ToString(),
                HttpRedirect = SimpleTemplate.Eval(Arg.HttpRedirest, new Dictionary<string, string> { { "DepositId", record.Id.ToString() } }),
                CallbackName = "SP.Accounting.Deposit.Done",
                CallbackContext = record.Id.ToString()
            });
            return record.Id;
        }
        public async Task<DepositStartResult> StartDeposit(int Id, Biz.Payments.RequestInfo RequestInfo)
		{
            var record = await Context.Find<TDepositRecord>(Id);
            if (record == null)
                throw new ArgumentException("充值记录不存在");
            if (record.State != DepositState.New)
                throw new ArgumentException("充值记录状态错误:" + record.State + ":" + record.Id);

            var ident = Ident.CreateDepositIdent(record.CreatedTime, record.DstId.ToString(), record.Id);
            try
            {
                var re = await CollectService.Value.Start(ident, RequestInfo);
                record.State = DepositState.Processing;
                Context.Update(record);
                await Context.SaveChangesAsync();

                return new DepositStartResult
                {
                    PaymentStartResult = re,
                    RecordId = ident,
                    Id=record.Id
                };
            }catch(Exception e)
            {
                await CompleteDeposit(record.Id, "启动收款失败", e);
                throw;
            }
		}
        public async Task<DepositRecord> RefreshDepositRecord(int Id,int DstId)
        {
            var s = await Context.Editable<TDepositRecord>()
                .Where(r=>r.Id==Id && r.DstId==r.DstId)
                .Select(r=>new { state = r.State ,time= r.CreatedTime })
                .SingleOrDefaultAsync();
            if (s == null)
            {
                Logger.Warn("充值记录不存在,ID:" + Id);
                return null;
            }
            if (s.state != DepositState.Processing)
                return await MapDepositRecord(Context.ReadOnly<TDepositRecord>().Where(r => r.Id == Id))
                    .SingleOrDefaultAsync();
            var ident = Ident.CreateDepositIdent(s.time, DstId.ToString(), Id);
            await this.CollectService.Value.TryCompleteByQuery(ident);
            await Task.Delay(3000);
            ((Data.Entity.DataContext)Context).Reset();
            return await MapDepositRecord(Context.ReadOnly<TDepositRecord>().Where(r => r.Id == Id))
                   .SingleOrDefaultAsync();

        }
        public async Task CompleteDeposit(int RecordId,string Desc, Exception exp)
		{
			var record = await Context.Editable<TDepositRecord>().FindAsync(RecordId);
            if (record == null)
            {
                Logger.Warn("充值记录不存在,ID:" + RecordId);
                return;
            }
            if(record.State==DepositState.Refunding ||
                record.State == DepositState.Refunded
                )
            {
                return;
            }
            if (record.State == DepositState.Processing)
            {

                if (exp == null)
                {
                    var acc = await Context.Editable<TAccount>()
                    .Where(a => a.AccountTitleId == record.AccountTitleId && a.OwnerId.Equals(record.DstId))
                    .SingleOrDefaultAsync();

                    if (acc == null)
                        acc = Context.Add(new TAccount
                        {
                            OwnerId = record.DstId,
                            AccountTitleId = record.AccountTitleId,
                            Inbound = record.Amount,
                            Outbound = 0,
                            CurValue = record.Amount,
                            UpdatedTime = TimeService.Now
                        });
                    else
                    {
                        acc.Inbound += record.Amount;
                        acc.CurValue = acc.Inbound - acc.Outbound;
                        acc.UpdatedTime = TimeService.Now;
                    }
                    record.CurValue = acc.CurValue;
                }

                record.State = exp == null ? DepositState.Completed : DepositState.Failed;
                record.CompletedTime = TimeService.Now;
                record.PaymentDesc = Desc;
                record.Error = exp?.Message.Limit(200);
                await Context.SaveChangesAsync();

                if (exp == null)
                {
                    var titleIdent = await Context
                        .ReadOnly<TTitle>()
                        .Where(t => t.Id == record.AccountTitleId)
                        .Select(t => t.Ident)
                        .SingleOrDefaultAsync();

                    //var e = Event.Create("账户", "充值完成", Tuple.Create(RecordId, record.DstId, record.Amount, Desc));
                    var e = new AccountDepositCompletedEvent(
                        RecordId, 
                        record.DstId, 
                        Desc, 
                        record.Amount,
                        titleIdent
                        );
                    await EventEmitter.Value.Emit(e, true);
                    await EventEmitter.Value.Emit(e, false);
                }
            }
            else
                Logger.Warn($"充值已处理,ID:{RecordId} 状态：{record.State}");
            
            //总是提交回调
			if (record.CallbackName != null)
				await CallGuarantor.Value.Schedule(
					record.CallbackName,
					record.CallbackContext,
					record.Id.ToString(),
					exp,
					"充值操作已完成",
					DateTime.MinValue,
					0,
					60
					);

        }

        public virtual IContextQueryable<TDepositRecordPublic> MapDepositRecord(IContextQueryable<TDepositRecord> query)
        {
            return from r in query
                   select new TDepositRecordPublic
                   {
                       Id = r.Id,
                       CreatedTime = r.CreatedTime,
                       DstId = r.DstId,
                       Amount = r.Amount,
                       CurValue=r.CurValue,
                       Title = r.Title,
                       AccountTitleId = r.AccountTitleId,
                       AccountTitle=r.AccountTitle.Title,
                       CompletedTime = r.CompletedTime,
                       State = r.State,
                       PaymentDesc = r.PaymentDesc,
                       PaymentPlatformId = r.PaymentPlatformId,
                       TrackEntityIdent=r.TrackEntityIdent,
                       Error = r.Error,
                       RefundRequest=r.RefundRequest,
                       RefundSuccess = r.RefundSuccess,
                       LastRefundRequestTime = r.LastRefundRequestTime,
                       LastRefundSuccessTime = r.LastRefundSuccessTime,
                       LastRefundReason = r.LastRefundReason,

                       OpAddress =r.OpAddress,
                       OpDevice=r.OpDevice
                   };
        }
       
        static PagingQueryBuilder<TDepositRecord> DepositRecordPageBuilder = new PagingQueryBuilder<TDepositRecord>(
            "time",
            i => i.Add("time", r => r.CreatedTime, true)
            );
        class DepositSummary : ISummaryWithCount
        {
            public int Count { get; set; }
            public decimal 金额 { get; set; }
        }
        public async Task<QueryResult<DepositRecord>> ListDepositRecord(
            DepositRecordQueryArguments args,
            Paging Paging
            )
        {
            if (!string.IsNullOrWhiteSpace(args.Ident))
            {
                int id = Accounting.Ident.ParseDepositIdent(args.Ident.Trim());
                if (id == 0)
                    return QueryResult<DepositRecord>.Empty;
                var re = await GetDepositRecord(id);
                if (re == null || args.OwnerId.HasValue && !re.DstId.Equals(args.OwnerId.Value))
                    return QueryResult<DepositRecord>.Empty;
                return new QueryResult<DepositRecord>
                {
                    Total = 1,
                    Items = new[] { re }
                };
            }

            IContextQueryable<TDepositRecord> q = Context.ReadOnly<TDepositRecord>();
            q = q.Filter(args.OwnerId, r => r.DstId)
                .Filter(args.TitleId, r => r.AccountTitleId)
                .Filter(args.PaymentPlatformId, r => r.PaymentPlatformId)
                .Filter(args.Time, r => r.CreatedTime)
                .Filter(args.Amount,r=>r.Amount)
                ;

            if (args.State.HasValue)
            {
                if (args.State.Value == DepositState.AfterProcessing)
                {
                    q = q.Where(r => 
                        r.State == DepositState.Completed ||
                        r.State == DepositState.Refunded ||
                        r.State == DepositState.Refunding || 
                        r.State == DepositState.Failed
                        );
                }
                else if(args.State.Value== DepositState.CompletedWithRefund)
                {
                    q = q.Where(r =>
                        r.State == DepositState.Completed ||
                        r.State == DepositState.Refunded ||
                        r.State == DepositState.Refunding
                        );
                }
                else
                    q = q.Where(r => r.State == args.State.Value);
            }


            var result= await q.ToQueryResultAsync(
                MapDepositRecord,
                r => (DepositRecord)r,
                DepositRecordPageBuilder,
                Paging,
                g=>new DepositSummary
                {
                    Count=g.Count(),
                    金额=g.Select(i=>i.Amount).DefaultIfEmpty().Sum()
                }
                );
            await PrepareDepositRecords(result.Items);
            return result;
        }
        public async Task<DepositRecord> GetDepositRecord(int Id)
        {
            var q = Context.ReadOnly<TDepositRecord>()
                    .Where(r => r.Id == Id)
                  ;
            var re= await MapDepositRecord(q).SingleOrDefaultAsync();
            if (re != null)
                await PrepareDepositRecords(new[] { re });
            return re;
        }
        async Task PrepareDepositRecords(IEnumerable<DepositRecord> records)
        {
            await ObjectResolver.Value.Fill(
                records, 
                r =>new[] {
                    "用户-" +r.DstId,
                    "系统服务-"+r.PaymentPlatformId,
                },
                (r, ns) =>
                {
                    r.DstName = ns[0] ?? "未知用户";
                    r.PaymentPlatformName = ns[1] ??"未知平台";
                });
        }
        #endregion




        #region 退款
        public async Task<int> CreateRefund(RefundArgument Arg)
        {
            if(Arg.Amount<=0)
                throw new ArgumentException("退款金额必须大于0");

            var title = await Context.ReadOnly<TTitle>()
                .Where(t => t.Ident == Arg.AccountTitle)
                .Select(t => t.Id).SingleOrDefaultAsync();
            if (title == 0)
                throw new ArgumentException("账户科目不存在:" + Arg.AccountTitle);

            var depositRecord = await Context.Find<TDepositRecord>(Arg.DepositRecordId);
            if (depositRecord == null)
                throw new ArgumentException("找不到充值记录");
            if(depositRecord.State!=DepositState.Completed &&
                depositRecord.State != DepositState.Refunded &&
                depositRecord.State != DepositState.Refunding
                )
                throw new ArgumentException("充值未成功，不能退款");

            var refundedAmount = await Context.ReadOnly<TRefundRecord>()
                .Where(r => r.DepositRecordId == Arg.DepositRecordId)
                .Select(r=>r.Amount)
                .DefaultIfEmpty(0)
                .SumAsync();

            if (refundedAmount+Arg.Amount > depositRecord.Amount)
                throw new ArgumentException("退款金额已超出充值未退款金额");

            if(depositRecord.RefundRequest+Arg.Amount>depositRecord.Amount)
                throw new ArgumentException("退款金额已超出充值未退款金额");

            var time = TimeService.Now;
            depositRecord.State = DepositState.Refunding;
            depositRecord.RefundRequest += Arg.Amount;
            depositRecord.LastRefundRequestTime = time;
            depositRecord.LastRefundReason = Arg.Reason.Limit(100);
            Context.Update(depositRecord);
            
            var record = Context.Add(new TRefundRecord
            {
                DepositRecordId=Arg.DepositRecordId,
                AccountTitleId = title,
                SrcId = depositRecord.DstId,
                OperatorId = Arg.OperatorId,
                Amount = Arg.Amount,
                Title = Arg.Description.Limit(200),
                TrackEntityIdent = Arg.TrackEntityIdent,
                CreatedTime = time,
                CallbackName = Arg.CallbackName,
                CallbackContext = Arg.CallbackContext,
                PaymentPlatformId = depositRecord.PaymentPlatformId,
                State = RefundState.Processing,
                DepositRecordCreateTime= depositRecord.CreatedTime,
                Reason=Arg.Reason.Limit(100)
            });
            await Context.SaveChangesAsync();
            var ident = Ident.CreateRefundIdent(
                time,
                depositRecord.DstId.ToString(),
                record.Id
                );

            var refundService = (Payments.IRefundService)CollectService.Value;
            await refundService.Create(new Payments.RefundRequest
            {
                PaymentPlatformId=depositRecord.PaymentPlatformId,
                Amount = Arg.Amount,
                Desc = Arg.Description,
                Title = Arg.Description,
                SubmitTime = time,
                CollectIdent = Ident.CreateDepositIdent(depositRecord.CreatedTime, depositRecord.DstId.ToString(), depositRecord.Id),
                Ident = ident,
                OpAddress = Arg.OpAddress,
                OpDevice = Arg.OpDevice,
                TrackEntityIdent = "账户退款记录-" + record.Id,
                CurUserId = depositRecord.DstId.ToString()
            });
            return record.Id;
        }

        public async Task<RefundState> RefreshRefundRecord(int Id, int DstId)
        {

            var record = await Context.Find<TRefundRecord>(Id);
            if (record.State == RefundState.Error || record.State == RefundState.Success)
                return record.State;

            
            var orgState = record.State;
            var time = TimeService.Now;
            var refundService = (Payments.IRefundService)CollectService.Value;

            var refundIdent = Ident.CreateRefundIdent(record.CreatedTime, record.SrcId.ToString(), record.Id);
            try
            {
                var result = await refundService.RefreshRefundRecord(refundIdent);
                switch (result.State)
                {
                    case Payments.PaymentRefundState.Failed:
                        record.CompletedTime = result.UpdatedTime;
                        record.State = RefundState.Error;
                        break;
                    case Payments.PaymentRefundState.Processing:
                        //旧通知，忽略
                        if (record.State == RefundState.Sending)
                            record.SubmittedTime = result.UpdatedTime;
                        record.State = RefundState.Processing;
                        break;
                    case Payments.PaymentRefundState.Submitting:
                        record.State = RefundState.Sending;
                        break;
                    case Payments.PaymentRefundState.Success:
                        record.State = RefundState.Success;
                        record.CompletedTime = result.UpdatedTime;
                        break;
                    default:
                        throw new ArgumentException("不支持支付退款状态：" + result.State);
                }

                record.UpdatedTime = time;

                if (result.State == Payments.PaymentRefundState.Success)
                {
                    var acc = await Context.Editable<TAccount>()
                        .Where(a => a.AccountTitleId == record.AccountTitleId && a.OwnerId.Equals(record.SrcId))
                        .SingleOrDefaultAsync();

                    if (acc == null)
                        acc = Context.Add(new TAccount
                        {
                            OwnerId = record.SrcId,
                            AccountTitleId = record.AccountTitleId,
                            Inbound = 0,
                            Outbound = record.Amount,
                            CurValue = -record.Amount,
                            UpdatedTime = time
                        });
                    else
                    {
                        acc.Outbound += record.Amount;
                        acc.CurValue = acc.Inbound - acc.Outbound;
                        acc.UpdatedTime = time;
                    }
                    record.CurValue = acc.CurValue;


                    var depositRecord = await Context.Find<TDepositRecord>(record.DepositRecordId);
                    depositRecord.RefundSuccess += depositRecord.Amount;
                    depositRecord.LastRefundSuccessTime = time;
                    if (depositRecord.RefundSuccess >= depositRecord.RefundRequest)
                        depositRecord.State = DepositState.Refunded;
                    Context.Update(depositRecord);
                }

                record.PaymentDesc = result.Desc;
                record.Error = result.Error;
                record.UpdatedTime = result.UpdatedTime;

            }
            catch (Exception exp)
            {
                record.Error = exp?.Message.Limit(200);
            }
            Context.Update(record);
            await Context.SaveChangesAsync();

            Logger.Warn($"退款状态更新,ID:{record.Id} 状态：{record.State} 错误:{record.Error}");
            if (record.State == RefundState.Error || record.State == RefundState.Success)
            {
                //var e = Event.Create("账户", "充值完成", Tuple.Create(RecordId, record.DstId, record.Amount, Desc));
                var e = new AccountRefundCompletedEvent(
                    record.Id,
                    record.SrcId,
                    record.PaymentDesc,
                    record.Amount,
                    record.State == RefundState.Success
                    );
                await EventEmitter.Value.Emit(e, true);
                await EventEmitter.Value.Emit(e, false);
            }
                
            return record.State;

            ////总是提交回调
            //if ((record.State == RefundState.Error || record.State == RefundState.Success)
            //    && record.CallbackName != null)
            //    await CallGuarantor.Value.Schedule(
            //        record.CallbackName,
            //        record.CallbackContext,
            //        record.Id.ToString(),
            //        exp,
            //        "退款操作已完成",
            //        DateTime.MinValue,
            //        0,
            //        60
            //        );




        }






        public virtual IContextQueryable<TRefundRecordPublic> MapRefundRecord(IContextQueryable<TRefundRecord> query)
        {
            return from r in query
                   select new TRefundRecordPublic
                   {
                       Id = r.Id,
                       DepositRecordId=r.DepositRecordId,
                       DepositRecordCreateTime=r.DepositRecordCreateTime,
                       CreatedTime = r.CreatedTime,
                       SrcId = r.SrcId,

                       Amount = r.Amount,
                       CurValue=r.CurValue,
                       Title = r.Title,
                       AccountTitleId = r.AccountTitleId,
                       AccountTitle = r.AccountTitle.Title,
                       CompletedTime = r.CompletedTime,
                       State = r.State,
                       PaymentDesc = r.PaymentDesc,
                       PaymentPlatformId = r.PaymentPlatformId,
                       TrackEntityIdent = r.TrackEntityIdent,
                       Error = r.Error,
                       Reason=r.Reason,
                       OpAddress = r.OpAddress,
                       OpDevice = r.OpDevice
                   };
        }
        static PagingQueryBuilder<TRefundRecord> RefundRecordPageBuilder = new PagingQueryBuilder<TRefundRecord>(
            "time",
            i => i.Add("time", r => r.CreatedTime, true)
            );
        class RefundSummary : ISummaryWithCount
        {
            public int Count { get; set; }
            public decimal 金额 { get; set; }
        }
        public async Task<QueryResult<RefundRecord>> ListRefundRecord(
            RefundRecordQueryArguments args,
            Paging Paging
            )
        {
            if (!string.IsNullOrWhiteSpace(args.RefundIdent))
            {
                int id = Accounting.Ident.ParseRefundIdent(args.RefundIdent.Trim());
                if (id == 0)
                    return QueryResult<RefundRecord>.Empty;
                var re = await GetRefundRecord(id);
                if (re == null || args.OwnerId.HasValue && !re.SrcId.Equals(args.OwnerId.Value))
                    return QueryResult<RefundRecord>.Empty;
                return new QueryResult<RefundRecord>
                {
                    Total = 1,
                    Items = new[] { re }
                };
            }


            IContextQueryable<TRefundRecord> q = Context.ReadOnly<TRefundRecord>();

            if (!string.IsNullOrWhiteSpace(args.DepositIdent))
            {
                var di = Ident.ParseDepositIdent(args.DepositIdent.Trim());
                q = q.Filter(di, r => r.DepositRecordId);
            }
            q = q.Filter(args.OwnerId, r => r.SrcId)
                .Filter(args.TitleId, r => r.AccountTitleId)
                .Filter(args.State, r => r.State)
                .Filter(args.PaymentPlatformId, r => r.PaymentPlatformId)
                .Filter(args.Time, r => r.CreatedTime)
                .Filter(args.Amount,r=>r.Amount)
                ;

            var result = await q.ToQueryResultAsync(
                MapRefundRecord,
                r => (RefundRecord)r,
                RefundRecordPageBuilder,
                Paging,
                g=>new RefundSummary
                {
                    Count=g.Count(),
                    金额=g.Select(i=>i.Amount).DefaultIfEmpty().Sum()
                }
                );
            await PrepareRefundRecords(result.Items);
            return result;
        }
        public async Task<RefundRecord> GetRefundRecord(int Id)
        {
            var q = Context.ReadOnly<TRefundRecord>()
                    .Where(r => r.Id == Id)
                  ;
            var re = await MapRefundRecord(q).SingleOrDefaultAsync();
            if (re != null)
                await PrepareRefundRecords(new[] { re });
            return re;
        }
        async Task PrepareRefundRecords(IEnumerable<RefundRecord> records)
        {
            await ObjectResolver.Value.Fill(
                records,
                r => new[] {
                    "用户-" +r.SrcId,
                    "系统服务-"+r.PaymentPlatformId,
                },
                (r, ns) =>
                {
                    r.SrcName = ns[0] ?? "未知用户";
                    r.PaymentPlatformName = ns[1] ?? "未知平台";
                });
        }
        #endregion

        #region 转账


        decimal UpdateAccount(
            Dictionary<KeyValuePair<int, int>, TAccount> accounts,
            int TitleId,
            int OwnerId,
            decimal InboundDiff,
            decimal OutboundDiff,
            DateTime Time
            )
        {
            //系统账号
            if (OwnerId == 0)
                return 0;

            TAccount acc;
            var key = new KeyValuePair<int, int>(TitleId, OwnerId);
            if (!accounts.TryGetValue(key, out acc))
            {
                acc = new TAccount
                {
                    AccountTitleId = TitleId,
                    OwnerId = OwnerId
                };
                Context.Add(acc);
                accounts.Add(key, acc);
            }
            else
                Context.Update(acc);

            var newInbound = acc.Inbound + InboundDiff;
            var newOutbound = acc.Outbound + OutboundDiff;
            if (newOutbound > newInbound)
                throw new BalanceNotEnough(acc.Inbound - acc.Outbound, OutboundDiff- InboundDiff);

            acc.Inbound = newInbound;
            acc.Outbound = newOutbound;
            acc.CurValue = newInbound - newOutbound;
            acc.UpdatedTime = Time;
            return acc.CurValue;
        }
        public async Task<int[]> Settlement(SettlementArgument Arg)
        {
            var accs = await (from a in Context.ReadOnly<TAccount>()
                           join t in Context.ReadOnly<TTitle>() on a.AccountTitleId equals t.Id
                           where a.OwnerId == Arg.SrcId && (t.Ident==Arg.FirstTitle || t.SettlementEnabled) && a.Inbound>a.Outbound
                           orderby t.SettlementOrder
                           select new { amount = a.Inbound - a.Outbound, title = t.Ident }
                    ).ToArrayAsync();
            if (accs.Length == 0)
                throw new BalanceNotEnough(0, Arg.Amount);
            var re = accs.Aggregate(
                new { idx = 0, amount = Arg.Amount, items = new List<TransferArgumentItem>() },
                (c, acc) =>
                {
                    if (c.amount == 0)
                        return c;
                    var item = new TransferArgumentItem
                    {
                        Amount = Math.Min(c.amount, acc.amount),
                        TrackEntityIdent = Arg.TraceEntityIdent,
                       
                        BizRecordIndex = c.idx,
                        Description = Arg.Description,
                        DstId = Arg.DstId,
                        DstTitle = Arg.DstTitle,
                        SrcId = Arg.SrcId,
                        SrcTitle = acc.title,
                    };
                    c.items.Add(item);
                    return new { idx = c.idx + 1, amount = c.amount - item.Amount, items = c.items };
                });
            if (re.amount > 0)
                throw new BalanceNotEnough(Arg.Amount - re.amount, Arg.Amount);
            return await Transfer(new TransferArgument
            {
                OperatorId = Arg.OperatorId,
                Items = re.items.ToArray(),
                OpAddress=Arg.OpAddress,
                OpDevice=Arg.OpDevice
            });
        }
        public async Task<int[]> Transfer(TransferArgument Arg)
        {
            var titles = await Context.ReadOnly<TTitle>()
                .Select(t => new { key = t.Ident, value = t.Id })
                .ToDictionaryAsync(t => t.key, t => t.value);

            var acc_groups = Arg.Items.SelectMany(i => new[] {
                         new KeyValuePair<int,int>(titles[i.DstTitle],i.DstId),
                         new KeyValuePair<int,int>(titles[i.SrcTitle],i.SrcId)
                     }).GroupBy(p => p.Key, p => p.Value);

            var accs = new Dictionary<KeyValuePair<int, int>, TAccount>();
            foreach (var grp in acc_groups)
            {
                var cid = grp.Key;
                var oids = grp.Where(id => id != 0).ToArray();
                var re = await Context.Editable<TAccount>()
                    .Where(a => a.AccountTitleId == cid && oids.Contains(a.OwnerId))
                    .ToArrayAsync();
                foreach (var a in re)
                    accs.Add(new KeyValuePair<int, int>(cid, a.OwnerId), a);
            }

            var time = TimeService.Now;
            var trs = new List<TTransferRecord>();
            foreach (var item in Arg.Items)
            {
                var srcCurValue=UpdateAccount(accs, titles[item.SrcTitle], item.SrcId, 0, item.Amount, time);
                var dstCurValue=UpdateAccount(accs, titles[item.DstTitle], item.DstId, item.Amount, 0, time);
                trs.Add(Context.Add(new TTransferRecord
                {
                    SrcTitleId = titles[item.SrcTitle],
                    DstTitleId = titles[item.DstTitle],
                    SrcId = item.SrcId,
                    DstId = item.DstId,
                    Time = time,
                    Amount = item.Amount,
                    SrcCurValue=srcCurValue,
                    DstCurValue=dstCurValue,
                    Title = item.Description.Limit(200),
                    TrackEntityIdent = item.TrackEntityIdent,
                    BizRecordIndex=item.BizRecordIndex
                }));
            }
            //var rec = BuildTransferRecord(titles,Arg.OperatorId, time, Arg.TopItem);
            await Context.SaveChangesAsync();
            return trs.Select(t => t.Id).ToArray();
        }


        public virtual IContextQueryable<TTransferRecordPublic> MapTransferRecord(IContextQueryable<TTransferRecord> query)
		{
			return from r in query
				   select new TTransferRecordPublic
				   {

                       Id=r.Id,
                       SrcTitleId=r.SrcTitleId,
                       DstTitleId=r.DstTitleId,
                       SrcTitleName=r.SrcAccountTitle.Title,
                       DstTitleName=r.DstAccountTitle.Title,

                        SrcId=r.SrcId,
                        DstId=r.DstId,
                        
                        Title=r.Title,
                        OpDevice=r.OpDevice,
                        OpAddress=r.OpAddress,
                        TrackEntityIdent=r.TrackEntityIdent,
                        Time=r.Time,

                        Amount=r.Amount,
                        DstCurValue=r.DstCurValue,
                        SrcCurValue=r.SrcCurValue,
                   };
		}
		static PagingQueryBuilder<TTransferRecord> TransferRecordPageBuilder = new PagingQueryBuilder<TTransferRecord>(
			"time",
			i => i.Add("time", r => r.Time, true)
			);
        class TransferSummary :ISummaryWithCount
        {
            public int Count { get; set; }
            public decimal 数额 { get; set; }
        }
		public async Task<QueryResult<TransferRecord>> ListTransferRecord(
            TransferRecordQueryArguments args,
            Paging Paging
			)
		{
			if(args.Id.HasValue)
			{
				var re = await GetTransferRecord(args.Id.Value);
				if(re==null)
					return QueryResult<TransferRecord>.Empty;
				return new QueryResult<TransferRecord>
				{
					Total = 1,
					Items = new[] { re }
				};
			}

            IContextQueryable<TTransferRecord> q = Context.ReadOnly<TTransferRecord>();
            q = q.Filter(args.SrcId, r => r.SrcId)
                .Filter(args.DstId, r => r.DstId)
                .Filter(args.SrcTitleId, r => r.SrcTitleId)
                .Filter(args.DstTitleId, r => r.DstTitleId)
                .Filter(args.Time, r => r.Time)
                .Filter(args.Amount,r=>r.Amount)
                ;

			var result= await q.ToQueryResultAsync(
				MapTransferRecord,
				r => (TransferRecord)r,
				TransferRecordPageBuilder,
				Paging,
                g=>new TransferSummary
                {
                    Count=g.Count(),
                    数额=g.Select(i=>i.Amount).DefaultIfEmpty().Sum()
                }

                );
            await FillTransferRecordUserName(result.Items);
            return result;

		}
		public async Task<TransferRecord> GetTransferRecord(int Id)
		{
			var q = Context.ReadOnly<TTransferRecord>()
					.Where(r => r.Id==Id)
				  ;
			var re=await MapTransferRecord(q).SingleOrDefaultAsync();
            if (re == null)
                return null;
            await FillTransferRecordUserName(new[] { re });
            return re;
		}
        async Task FillTransferRecordUserName(IEnumerable<TransferRecord> records)
        {
            await ObjectResolver.Value.Fill(
                records, 
                r => new[] {
                    "用户-"+r.SrcId,
                    "用户-"+r.DstId
                }, 
                (r, us) =>
                   {
                       r.SrcName = us[0] ?? (r.SrcId == 0 ? "系统" : "未知用户");
                       r.DstName = us[1] ?? (r.DstId == 0 ? "系统" : "未知用户");
                   });
        }

        #endregion

        #region 账户
        public async Task<int> GetTitleId(string Title)
        {
            if (Title == null)
                Title = "balance";
            var re=await Context.ReadOnly<TTitle>()
                .Where(t => t.Ident == Title)
                .Select(t => t.Id)
                .SingleOrDefaultAsync();
            return re;
        }

        public async Task<Account> GetAccount(int CaptionId, int OwnerId)
        {
            var re = await Context.ReadOnly<TAccount>()
                .Where(a => a.AccountTitleId == CaptionId && a.OwnerId.Equals(OwnerId))
                .Select(a => new TAccountPublic
                {
                    Inbound = a.Inbound,
                    Outbound = a.Outbound,
                })
                .SingleOrDefaultAsync();
            return re ?? new TAccountPublic();
        }
        public async Task<decimal> GetTitleValue(int OwnerId, string Title)
		{
			if (string.IsNullOrEmpty(Title))
				Title = "balance";
			var q = from a in Context.ReadOnly<TAccount>()
					join t in Context.ReadOnly<TTitle>() on a.AccountTitleId equals t.Id
					where t.Ident == Title && a.OwnerId.Equals(OwnerId)
					select a.Inbound - a.Outbound;

			return await q.SingleOrDefaultAsync();
		}
        public async Task<decimal> GetSettlementBalance(int OwnerId)
        {
            var q = from a in Context.ReadOnly<TAccount>()
                    join t in Context.ReadOnly<TTitle>() on a.AccountTitleId equals t.Id
                    where a.OwnerId==OwnerId && t.SettlementEnabled
                    select a.Inbound - a.Outbound;
            return await q.DefaultIfEmpty(0).SumAsync();
        }
        public async Task<Dictionary<int,decimal>> GetSettlementBalances(int[] OwnerIds)
        {
            var q = from a in Context.ReadOnly<TAccount>()
                    join t in Context.ReadOnly<TTitle>() on a.AccountTitleId equals t.Id
                    where OwnerIds.Contains(a.OwnerId) && t.SettlementEnabled
                    group a by a.OwnerId into g
                    select new { id = g.Key, balance = g.Select(i => i.Inbound - i.Outbound).DefaultIfEmpty().Sum() };
            return await q.ToDictionaryAsync(i => i.id, i => i.balance);
        }

        public virtual IContextQueryable<TAccountInternal> MapAccountInternal(IContextQueryable<TAccount> query)
        {
            return from r in query
                   select new TAccountInternal
                   {
                        OwnerId=r.OwnerId,
                        TitleId=r.AccountTitleId,
                        TitleName=r.AccountTitle.Title,
                        Amount=r.Inbound-r.Outbound,
                        Inbound=r.Inbound,
                        Outbound=r.Outbound,
                        UpdatedTime=r.UpdatedTime
                 };
        }
        static PagingQueryBuilder<TAccount> AccountInternalPageBuilder = new PagingQueryBuilder<TAccount>(
            "owner",
            i => i.Add("owner", r => r.OwnerId, true)
            );
        public async Task<AccountInternal> GetAccountInternal(int TitleId, int OwnerId)
        {
            var q = Context.ReadOnly<TAccount>().Where(r => r.AccountTitleId == TitleId && r.OwnerId == OwnerId);
            var re=await MapAccountInternal(q).SingleOrDefaultAsync();
            if (re == null)
                return re;
            await FillAccountUserName(new[] { re });
            return re;
        }
        class AccountSummary : ISummaryWithCount
        {
            public int Count { get; set; }
            public decimal 余额 { get; set; }
            public decimal 转入 { get; set; }
            public decimal 转出 { get; set; }
        }
        public async Task<QueryResult<AccountInternal>> ListAccountInternals(
            AccountQueryArguments args,
            Paging Paging
            )
        {
           
            IContextQueryable<TAccount> q = Context.ReadOnly<TAccount>();
            q = q.Filter(args.OwnerId, r => r.OwnerId)
                .Filter(args.TitleId, r => r.AccountTitleId)
                .Filter(args.UpdatedTime, r => r.UpdatedTime)
                .Filter(args.Amount, r => r.Inbound - r.Outbound)
                .Filter(args.Inbound, r => r.Inbound)
                .Filter(args.Outbound, r => r.Outbound)
                ;
            var re= await q.ToQueryResultAsync(
                MapAccountInternal,
                r => (AccountInternal)r,
                AccountInternalPageBuilder,
                Paging,
                g=>new AccountSummary
                {
                    Count=g.Count(),
                    余额=g.Select(i=>i.CurValue).DefaultIfEmpty().Sum(),
                    转入= g.Select(i => i.Inbound).DefaultIfEmpty().Sum(),
                    转出 = g.Select(i => i.Outbound).DefaultIfEmpty().Sum()
                }
                );
            await FillAccountUserName(re.Items);
            return re;

        }
        async Task FillAccountUserName(IEnumerable<AccountInternal> accounts)
        {
            await ObjectResolver.Value.Fill(
                accounts, 
                r => "用户-"+r.OwnerId, 
                (r, u) =>
                {
                    r.OwnerName = u ?? "未知用户";
                });
        }
        #endregion



        async Task<IDataObject[]> IDataObjectLoader.Load(string Type, string[][] Keys)
        {
            switch (Type)
            {
                case "账户充值记录":
                    var re = await DataObjectLoader.Load(
                        Keys,
                        id => int.Parse(id[0]),
                        id => GetDepositRecord(id),
                        async (ids) => {
                            var q = MapDepositRecord(
                                Context.ReadOnly<TDepositRecord>()
                                .Where(a => ids.Contains(a.Id))
                                );
                            var tmps = await q.ToArrayAsync();
                            return tmps.Cast<DepositRecord>().ToArray();
                        }
                        );
                    return re;
                default:
                    throw new NotSupportedException();
            }
            
        }
    }
}
