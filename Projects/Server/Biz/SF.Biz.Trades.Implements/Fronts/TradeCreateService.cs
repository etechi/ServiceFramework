using SF.Biz.Accounting;
using SF.Biz.Coupons;
using SF.Biz.Payments;
using SF.Biz.Products;
using SF.Biz.Trades.Managements;

using SF.Sys;
using SF.Sys.Auth;
using SF.Sys.Clients;
using SF.Sys.Data;
using SF.Sys.Entities;
using SF.Sys.Linq;
using SF.Sys.Logging;
using SF.Sys.Reminders;
using SF.Sys.TimeServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Biz.Trades
{
    public class TradeRemindable : IRemindable
    {
        ITradeCreateService TradeCreateService { get; }
        public TradeRemindable(ITradeCreateService TradeCreateService)
        {
            this.TradeCreateService = TradeCreateService;
        }
        public Task Remind(IRemindContext Context)
        {
            return TradeCreateService.UpdateStatus(Context.BizIdent);
        }
    }

    public class TradeCreateService:
        ITradeCreateService
	{
        public IDataScope DataScope { get; }
        public IAccessToken AccessToken { get; }
        long EnsureUserIdent() => AccessToken.User.EnsureUserIdent();

        IItemService ItemService { get; }
        IUserProfileService UserProfileService { get; }
        ITimeService TimeService { get; }
        IClientService ClientService { get; }
        ICouponService CouponService { get; }
        ITradeCreator TradeCreator { get; }
        Lazy<IDepositService> DepositService { get; }
        ILogger Logger { get; }
        Lazy<IItemNotifier> ProductItemNotifier { get;  }
        ITransferService TransferService { get; }

        public TradeCreateService(
            IDataScope DataScope, 
            IAccessToken AccessToken, 
            IItemService ItemService, 
            IUserProfileService UserProfileService,
            ITimeService TimeService,
            IClientService ClientService,
            ICouponService CouponService,
            ITradeCreator TradeCreator,
            Lazy<IDepositService> DepositService,
            ILogger<TradeCreateService> Logger,
            Lazy<IItemNotifier> ProductItemNotifier,
            ITransferService TransferService
            )
        {
            this.DataScope = DataScope;
            this.AccessToken = AccessToken;
            this.ItemService = ItemService;
            this.UserProfileService = UserProfileService;
            this.TimeService = TimeService;
            this.ClientService = ClientService;
            this.CouponService = CouponService;
            this.TradeCreator = TradeCreator;
            this.DepositService = DepositService;
            this.Logger = Logger;
            this.ProductItemNotifier = ProductItemNotifier;
            this.TransferService = TransferService;
        }

        async Task Settlement(IDataContext ctx,DataModels.DataTrade trade,long? DepositRecordId)
        {
            decimal totalAmount = 0;
            int totalQuantity = 0;
            var time = TimeService.Now;
            ICouponDiscountConfig couponConfig = null;
            CouponApplyArgument couponArgument = null;

            string Error = null;
            try
            {
                foreach (var it in trade.Items)
                {
                    var product = it.ProductId;
                    
                    product.SellCount += it.Quantity;
                    totalAmount += it.SettlementAmount;
                    totalQuantity += it.Quantity;

                    ctx.AddCommitTracker(TransactionCommitNotifyType.AfterCommit,(t,e)=>
                    {
                        ProductItemNotifier.Value.NotifyProductChanged(product.Id);
                        return Task.CompletedTask;
                    });

                }


                if (trade.DiscountEntityIdent != null)
                {
                    try
                    {
                        couponArgument = new CouponApplyArgument
                        {
                            Time = time,
                            Code = ObjectIdent.ParseSingleKey<string>(trade.DiscountEntityIdent, "优惠券"),
                            DstEntityId = "交易-" + trade.Id,
                            DstDesc = "交易:" + trade.Name,
                            OrgAmount = totalAmount,
                            UserId = trade.BuyerId,
                            Count = trade.DiscountEntityCount
                        };

                        couponConfig = await CouponService.GetCouponDiscountConfig(couponArgument);
                    }
                    catch (CouponException cex)
                    {
                        Error = cex.Message;
                    }
                }
                //Error = $"(第{8}期)中国电信50元话费充值卡剩余人次不足。";
            }
            catch (Exception ex)
            {
                Error = "订单结算失败";
                Logger.Error(ex, "未知订单结算异常:" + trade.Id);
            }

            if (Error != null)
                throw new TradeSettlementException(Error);

            //从这里开始Context会发生变化，不能再取消交易

            trade.State = TradeState.Closed;
            trade.EndTime = trade.LastStateTime = time;
            trade.EndType = TradeEndType.Completed;
                       
            trade.TotalAmount = totalAmount;
            trade.TotalQuantity = totalQuantity;
            trade.TotalSellerAmount =
            trade.TotalSettlementAmount =
                couponConfig == null ?
                totalAmount :
                await CouponService.Value.Apply(couponConfig, couponArgument);

            foreach (var it in trade.Items)
                ctx.Update(it);

            trade.CalcItemsApportionAmount(ctx);
            trade.SyncItemsState(ctx);
            ctx.Update(trade);
            await ctx.SaveChangesAsync();


            if (trade.TotalSettlementAmount > 0)
            {
                try
                {
                    await TransferService.Settlement(
                        new SettlementArgument
                        {
                            FirstTitle = DepositRecordId.HasValue ? "trade" : null,
                            OperatorId = trade.BuyerId,
                            Amount = trade.TotalSettlementAmount,
                            TraceEntityIdent = "交易-" + trade.Id,
                            Description = "订单结算:" + trade.Name,
                            DstId = 0,
                            DstTitle = trade.TradeType == TradeType.Normal ? "balance" : "simulation",
                            SrcId = trade.BuyerId,
                            OpDevice = trade.OpDevice,
                            OpAddress = trade.OpAddress
                        });
                }
                catch (BalanceNotEnough)
                {
                    throw new TradeSettlementException("余额不足");
                }
            }

            var tradeEvent = new TradeSettlementCompleted(
                trade.Id,
                trade.BuyerId,
                trade.SellerId,
                trade.Name,
                trade.TotalQuantity,
                trade.TotalSettlementAmount
                );

            //发送活动事件
            ServiceProtocol.Events.IEvent activityEvent = null;
            if (DepositRecordId.HasValue)
            {
                var rec = await Context.ReadOnly<DataModels.AccountDepositRecord>()
                    .Where(r => r.Id == DepositRecordId.Value)
                    .Select(r => new { amount = r.Amount, desc = r.PaymentDesc })
                    .SingleOrDefaultAsync();

                activityEvent = Promotions.PromotionSourceEventHandler.CreateEvent(
                    "账户充值完成",
                    trade.BuyerId,
                    "账户充值",
                    "账户充值记录-" + DepositRecordId,
                    rec.amount.Round(),
                    rec.desc
                    );
            }

            postActions.Add(async () =>
            {
                await EventEmitter.Value.Emit(tradeEvent, true);
                await EventEmitter.Value.Emit(tradeEvent, false);

                if (activityEvent != null)
                {
                    await EventEmitter.Value.Emit(activityEvent, true);
                    await EventEmitter.Value.Emit(activityEvent, false);
                }
            });
        }
        async Task<DataModels.DataTrade> LoadTrade(IDataContext ctx, long OrderId)
        {
            var trade = await ctx.Queryable<DataModels.DataTrade>()
                    .Where(t => t.Id == OrderId)
                    .Include(t => t.Items)
                    .SingleOrDefaultAsync();
            return trade;
        }

        void VerifyTradeDepositRecordId(DataModels.DataTrade trade, long? DepositRecordId)
        {
            if (!DepositRecordId.HasValue)
                return;
            if (!trade.DepositRecordId.HasValue)
                Logger.Error("充值结束的订单没有记录充值编号:" + trade.Id + " 充值Id:" + DepositRecordId);
            if (trade.DepositRecordId.Value != DepositRecordId)
                Logger.Error($"订单记录编号和实际返回的不一致：{trade.Id} 记录：{trade.DepositRecordId} 返回:{DepositRecordId}");
        }
        async Task<R> RetryOnDbDupKeyException<R>(IDataContext ctx,Func<Task<R>> callback)
        {
            //TODO: 临时代码，有性能问题
                return await ctx.RetryForConcurrencyException(async () =>
                {
                    return await ServiceProtocol.Functional.Retry(
                       callback,
                       e =>
                       {
                           if (!(e is DbDuplicatedKeyException))
                               return false;
                           var dc = Context as ServiceProtocol.Data.Entity.DataContext;
                           if (dc == null)
                               return false;
                           Logger.Warn("完成交易时发生主键冲突，一般是由于多个用户同时完成夺宝造成的，重在重新尝试");
                           dc.Reset();
                           return true;
                       },
                       5,
                       300
                       );
                }, 5, 300);
        }
        async Task SettlementWithTransactionAndCleanupShoppingCart(int TradeId, int? DepositRecordId)
        {
            try
            {
                var pair = await RetryOnDbDupKeyException(
                    async () =>
                    {
                        using (var trans = await TransactionScopeManager.Value.CreateScope("交易结算", TransactionScopeMode.RequireTransaction))
                        {
                            var t = await LoadTrade(TradeId);
                            if (t.EndType != TradeEndType.InProcessing)
                                return Tuple.Create(t, (PostActions)null);
                            VerifyTradeDepositRecordId(t, DepositRecordId);

                            var acts = new PostActions();
                            await Settlement(trans, t, acts, DepositRecordId);
                            await trans.Commit();
                            return Tuple.Create(t, acts);
                        }
                    });

                if (pair.Item2 != null)
                {
                    await pair.Item2.Execute();
                    //正常结束才清理购物车
                    await ClearShoppingCart(pair.Item1);
                }
            }
            catch (TradeSettlementException exp)
            {
                await Context.RetryForConcurrencyException(async () =>
                {
                    using (var trans = await TransactionScopeManager.Value.CreateScope(
                        "撤销订单",
                        TransactionScopeMode.RequireTransaction)
                        )
                    {
                        var t = await LoadTrade(TradeId);
                        await CancelTrade(trans, t, exp.Message, DepositRecordId);
                    }
                }, 5, 300);

                throw;
            }
        }


        async Task<(long TradeId, decimal Amount)> CreateInternal(
            long BuyerId,
            TradeCreateItem[] TradeItems,
            string CouponCode,
            int CouponCount
            )
        {
            if (TradeItems.Length == 0)
                throw new ArgumentException("没有购买项目");

            if (TradeItems.Select(ti => ti.ItemId).Distinct().Count() < TradeItems.Length)
                throw new PublicArgumentException("订单中有重复商品");
            
            var itemDicts = (await this.ItemService.GetItems(
                TradeItems.Select(ci => ci.ItemId).ToArray()
                )).ToDictionary(i => i.ItemId);

            var items = TradeItems.Select(ti => {
                if (!itemDicts.TryGetValue(ti.ItemId, out var item))
                    throw new PublicArgumentException("找不到商品:" + ti.ItemId);
                if (item.CouponDisabled && (CouponCount > 0 || CouponCode != null))
                    throw new PublicArgumentException("您的购物车中包含特殊商品，不能使用优惠券");
                return (item:item,tradeItem:ti);
                }).ToArray();


            var userids = items.Select(i => i.item.SellerId).Concat(EnumerableEx.From(BuyerId)).Distinct().ToArray();
            var users = (await UserProfileService.GetUsers(userids)).ToDictionary(u=>u.Id);
            if (users.Count != userids.Length)
                throw new ArgumentException("找不到部分用户：" + userids.Join(","));

            if(!users.TryGetValue(BuyerId,out var buyer))
                throw new ArgumentException("找不到买家用户：" + BuyerId);

            var seller = users.Values.FirstOrDefault(u => u != buyer);
            if(seller==null)
                throw new ArgumentException("找不到卖家用户");



            var maxAmountItem = items.OrderByDescending(i => i.tradeItem.Quantity * i.item.Price).Take(1).First();
            var amount = items.Sum(i => i.tradeItem.Quantity * i.item.Price);

            var args = new Managements.TradeCreateArgument
            {
                OpDevice = ClientService.UserAgent.DeviceType,
                OpAddress = ClientService.UserAgent.Address,
                BuyerId = BuyerId,
                BuyerName = buyer.Name,
                // TradeType = buyerType == UserType.Simulated ? TradeType.Simulated : TradeType.Normal,
                Image = maxAmountItem.item.Image,
                Name = maxAmountItem.item.Title + (items.Length > 1 ? "等" : ""),
                SettlementAmount = amount,
                Time = TimeService.Now,
                SellerId = seller.Id,
                SellerName = seller.Name,
                // DisableQuantityAdjuest = !AllowQuantityAdjuest,
                Items = items.Where(i => i.tradeItem.Quantity > 0).Select(i => new Managements.TradeItemCreateArgument
                {
                    Image = i.item.Image,
                    Price = i.item.Price,
                    ProductId = i.item.ItemId,
                    ProductType = "item",
                    Quantity = i.tradeItem.Quantity,
                    SettlementAmount = i.tradeItem.Quantity * i.item.Price,
                    SettlementPrice = i.item.Price,
                    Name = i.item.Title,

                }).ToArray()
            };

            if (CouponCode != null)
            {
                try
                {
                    var coupon = await CouponService.GetCouponDiscountConfig(
                        BuyerId,
                        CouponCode,
                        CouponCount,
                        amount
                        );

                    //此时的结算金额还是临时的，实际结算需要考虑实际能购买的数量
                    args.SettlementAmount = coupon.CalcNewAmount(
                        amount,
                        CouponCount,
                        1
                        ).NewAmount;
                    if (args.SettlementAmount < amount)
                    {
                        args.DiscountEntityId = "优惠券-" + CouponCode;
                        args.DiscountEntityCount = CouponCount;
                        args.DiscountDesc = /*"使用优惠券:" +*/ coupon.Name + " x " + CouponCount;
                    }
                }
                catch (CouponException cex)
                {
                    throw new TradeCreateException(cex.Message);
                }
            }

            var oid = await TradeCreator.Create(args);

            return (oid, args.SettlementAmount);
        }

        Task<TradeCreateResult> StartDepositForTrade(
            long BuyerId,
            long TradeId,
            decimal Amount,
            string ClientType,
            long DepositPaymentPlatformId,
            string HttpRedirect,
            StartRequestInfo RequestInfo
            )
        {
            return DataScope.Use("交易支付", async ctx =>
            {
                var trade = await ctx
                    .Queryable<DataModels.DataTrade>()
                    .Where(t => t.Id == TradeId)
                    .SingleOrDefaultAsync();

                var did = await DepositService.Value.Create(
                    new DepositArgument
                    {
                        AccountTitle = "trade",
                        Amount = Amount,
                        TrackEntityIdent = "交易-" + TradeId,
                        RemindId = 0,
                        ClientType = ClientType,
                        OpAddress = RequestInfo.ClientAddress,
                        OpDevice = RequestInfo.DeviceType,
                        Description = "充值:" + trade.Name,
                        DstId = BuyerId,
                        OperatorId = BuyerId,
                        PaymentPlatformId = DepositPaymentPlatformId,
                        HttpRedirest = HttpRedirect.Replace(new Dictionary<string, object> { { "TradeId", TradeId } })
                    });

                trade.DepositRecordId = did;
                ctx.Update(trade);
                await ctx.SaveChangesAsync();

                var deposit_result = await DepositService.Value.Start(did, RequestInfo);

                return new TradeCreateResult
                {
                    TradeId = TradeId,
                    Completed = false,
                    DepositStartResult = deposit_result.PaymentStartResult
                };
            });
        }

        public async Task<TradeCreateResult> Create(TradeCreateArgument arg)
        {
            //if (Setting.Value.TradeAdjuest)
            //  arg.AllowQuantityAdjuest = true;
            var BuyerId = EnsureUserIdent();

            var (tradeId, amount) = await CreateInternal(
                BuyerId,
                arg.Items,
                arg.CouponCode,
                arg.CouponCount
                );


            if (arg.DepositAmount > 0)
                return await StartDepositForTrade(
                    BuyerId,
                    tradeId,
                    arg.DepositAmount,
                    arg.ClientType,
                    arg.DepositPaymentPlatformId,
                    arg.HttpRedirect,
                    RequestInfo
                    );

            //在这里无法判断余额是否充足, 只能先尝试结算
            try
            {
                await SettlementWithTransactionAndCleanupShoppingCart(trade.Key, null);
                return new Models.DirectTradeStartResult
                {
                    TradeId = trade.Key,
                    Completed = true
                };
            }
            //catch(CouponException ce)
            //{
            //    var ctx = Context as ServiceProtocol.Data.Entity.DataContext;
            //    ctx.Reset();
            //    var tr= await LoadTrade(tradeId);
            //    CancelTrade(tr, "优惠券无效:" + ce.Message,null);
            //    await Context.SaveChangesAsync();
            //    throw;
            //}
            catch (BalanceNotEnough e)
            {
                ctx..Reset();
                return await StartDepositForTrade(
                    BuyerId,
                    trade.Key,
                    e.Required - e.Current,
                    arg.ClientType,
                    arg.DepositPaymentPlatform,
                    arg.HttpRedirect,
                    RequestInfo
                    );
            }
        }


	}
}
