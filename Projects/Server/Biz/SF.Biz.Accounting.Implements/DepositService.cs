using SF.Biz.Payments;
using SF.Sys.Data;
using SF.Sys.TimeServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Biz.Accounting
{
    public class DepositService:
		IDepositService
	{
        public IDataScope DataScope { get; }
        public ITimeService TimeService { get; }
        public ICollectService CollectService { get; }
        public DepositService(
            IDataScope DataScope,
            ITimeService TimeService,
            ICollectService CollectService
            )
        {
            this.DataScope = DataScope;
            this.TimeService = TimeService;
            this.CollectService = CollectService;
        }


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



    }
}
