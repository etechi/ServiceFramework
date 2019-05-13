using System;
using System.Linq;
using System.Threading.Tasks;
using SF.Sys;
using SF.Sys.Entities;
using SF.Sys.Data;

namespace SF.Biz.Trades.Managements
{

    public class TradeManager :
        AutoQueryableEntitySource<ObjectKey<long>, Trade, Trade, TradeQueryArguments, DataModels.DataTrade>,
        ITradeManager
    {
        ITradeSyncQueue SyncQueue { get; }
        public TradeManager(IEntityServiceContext ServiceContext, ITradeSyncQueue SyncQueue) : base(ServiceContext)
        {
            this.SyncQueue = SyncQueue;
        }

        void VerifyDiscount(
             string Name,
             decimal OrgValue,
             decimal SettlementValue,
             string DiscountDesc,
             string DiscountEntityId
             )
        {
            if (OrgValue == SettlementValue)
                return;
            if (OrgValue < SettlementValue)
                throw new ArgumentException($"结算{Name}不能比原始{Name}高");
            if (string.IsNullOrWhiteSpace(DiscountDesc))
                throw new ArgumentException($"结算{Name}比原始{Name}低时，必须提供折扣说明");
            if (string.IsNullOrWhiteSpace(DiscountEntityId))
                throw new ArgumentException($"结算{Name}比原始{Name}低时，必须提供折扣编号");
        }
        void VerifyRange(string Name, decimal Value, bool CanBeZero, decimal MaxValue)
        {
            if (CanBeZero)
            {
                if (Value < 0)
                    throw new ArgumentException($"{Name}不能小于0");
            }
            else if (Value <= 0)
                throw new ArgumentException($"{Name}不能小于等于0");

            if (Value > MaxValue)
                throw new ArgumentException($"{Name}不能超过{MaxValue}");
        }
        protected virtual void OnVerifyTradeItemArgument(
            TradeCreateArgument args,
            TradeItemCreateArgument TradeItem
            )
        {
            Ensure.HasContent(TradeItem.Name, "必须提供订单项目描述");
            Ensure.HasContent(TradeItem.ProductType, "必须指定产品类型");
            Ensure.Positive(TradeItem.ProductId, "必须指定产品ID");


            VerifyRange("数量", TradeItem.Quantity, false, 1000000);
            VerifyRange("价格金额", TradeItem.Price, true, 10000000);
            var OrgAmount = TradeItem.SettlementPrice * TradeItem.Quantity;
            VerifyRange("小计金额", OrgAmount, true, 10000000);
            VerifyRange("小计结算金额", TradeItem.SettlementAmount, true, 10000000);


            VerifyDiscount("单价金额", TradeItem.Price, TradeItem.SettlementPrice, TradeItem.PriceDiscountDesc, TradeItem.DiscountEntityIdent);

            VerifyDiscount("小计金额", OrgAmount, TradeItem.SettlementAmount, TradeItem.AmmountDiscountDesc, TradeItem.DiscountEntityIdent);

        }
        protected virtual void OnVerifyTradeArgument(TradeCreateArgument args)
        {
            Ensure.HasContent(args.Name, "必须提供订单描述");

            Ensure.NotDefault(args.SellerId, "必须提供卖家ID");
            Ensure.NotDefault(args.BuyerId, "必须提供买家ID");
            Ensure.NotDefault(args.SellerName, "必须提供卖家名称");
            Ensure.NotDefault(args.BuyerName, "必须提供买家名称");

            Ensure.NotNull(args.Items, "订单项");
            Ensure.Positive(args.Items.Length, "订单项数目");

            decimal amount = 0;
            var quantity = 0;
            foreach (var it in args.Items)
            {
                OnVerifyTradeItemArgument(args, it);
                amount += it.SettlementAmount;
                quantity += it.Quantity;
            }

            VerifyRange("总金额", amount, true, 10000000);
            VerifyRange("总结算金额", args.SettlementAmount, true, 10000000);
            VerifyDiscount("总金额", amount, args.SettlementAmount, args.DiscountDesc, args.DiscountEntityId);
        }

        protected virtual void OnInitTradeItem(DataModels.DataTradeItem Model, TradeItemCreateArgument item, TradeCreateArgument arg)
        {

        }
        protected virtual void OnInitTrade(DataModels.DataTrade Model, TradeCreateArgument arg)
        {

        }
        public async Task<long> Create(TradeCreateArgument args)
        {
            OnVerifyTradeArgument(args);

            var tid = await IdentGenerator.GenerateAsync<DataModels.DataTrade>();
            var iids = await IdentGenerator.GenerateAsync<DataModels.DataTradeItem>(args.Items.Length);
            var items = args.Items.Select((it, idx) =>
            {
                var item = new DataModels.DataTradeItem
                {
                    Id=iids[idx],
                    Order = idx,
                    Image = it.Image,
                    Name = it.Name,
                    ProductType = it.ProductType,
                    ProductId = it.ProductId,
                    SellerId = args.SellerId,
                    BuyerId = args.BuyerId,
                    Price = it.Price,
                    Quantity = it.Quantity,
                    PriceDiscountDesc = it.PriceDiscountDesc,
                    SettlementPrice = it.SettlementPrice,
                    Amount = it.Price * it.Quantity,
                    AmountDiscountDesc = it.AmmountDiscountDesc,
                    DiscountEntityIdent = it.DiscountEntityIdent,
                    SettlementAmount = it.SettlementAmount,
                    BuyerRemarks = it.BuyerRemarks,
                    SellerRemarks = it.SellerRemarks,
                    OpAddress = args.OpAddress,
                    OpDevice = args.OpDevice,
                    TradeType = args.TradeType,

                    State = TradeState.Created,
                    EndType = TradeEndType.InProcessing,
                    CreatedTime = args.Time,
                    LastStateTime = args.Time,
                };

                OnInitTradeItem(item, it, args);
                return item;
            }).ToArray();

            var trade = new DataModels.DataTrade
            {
                Id=tid,
                TotalAmount = items.Sum(it => it.Amount),
                TotalSettlementAmount = items.Sum(it => it.SettlementAmount),
                TotalQuantity = items.Sum(it => it.Quantity),
                SellerId = args.SellerId,
                BuyerId = args.BuyerId,
                OpAddress = args.OpAddress,
                OpDevice = args.OpDevice,
                TradeType = args.TradeType,
                State = TradeState.Created,
                EndType = TradeEndType.InProcessing,
                CreatedTime = args.Time,
                LastStateTime = args.Time,
                DiscountEntityIdent = args.DiscountEntityId,
                DiscountEntityCount = args.DiscountEntityCount,
                Image = args.Image,
                Name = args.Name,
                DiscountDesc = args.DiscountDesc,

                BuyerRemarks = args.Remarks,
                SellerRemarks = null,
                DeliveryAddressId = args.AddressId,
                SellerName = args.SellerName,
                BuyerName = args.BuyerName,
                Items = items
            };
            trade.CalcItemsApportionAmount();

            OnInitTrade(trade, args);

            return await DataScope.Use("创建订单", async ctx =>
            {
                ctx.Add(trade);
                await ctx.SaveChangesAsync();
                return trade.Id;
            });
        }


        protected class Result
        {
            public TradeState NewState { get; set; }
            public string EventName { get; set; }
        }
        protected virtual Task<DataModels.DataTrade> Process(
            long TradeId,
            TradeActionType ActionType,
            TradeState[] StateRequired,
            Func<DataModels.DataTrade, DateTime, Task<Result>> action
            )
        {
            return SyncQueue.Queue(
                TradeId,
                () => DataScope.Use("处理订单", async ctx =>
                {
                    var time = Now;
                    var trade = await ctx.Queryable<DataModels.DataTrade>()
                        .Where(t => t.Id.Equals(TradeId))
                        .Include(t => t.Items)
                        .SingleOrDefaultAsync();

                    if (trade == null)
                        throw new TradeException("订单不存在");

                    //if (ctx.pre_executor != null)
                    //await ctx.pre_executor(order);

                    if (!StateRequired.Contains(trade.State))
                        throw new TradeException($"订单状态错误，当前状态:{trade.State} 当前操作:{ActionType}");

                    var org_state = trade.State;
                    var org_end_type = trade.EndType;

                    //清除超时时间
                    trade.StateExpires = null;
                    var re = await action(trade, time);
                    trade.State = re.NewState;
                    trade.LastStateTime = time;
                    trade.SyncItemsState(ctx);
                    ctx.Update(trade);
                    await ctx.SaveChangesAsync();
                    return trade;
                })
               );
        }
        Result OK(TradeState state, string eventName)
        {
            return new Result { EventName = eventName, NewState = state };
        }

        void CalcAmount(DataModels.DataTrade trade, decimal PlatformPercent, decimal SellerPercent)
        {
            var buyer_amount = trade.TotalSettlementAmount;
            if (PlatformPercent + SellerPercent > 1)
                throw new TradeException("金额比例错误");

            trade.PlatformAmount = Math.Round(trade.TotalSettlementAmount * PlatformPercent, 2);

            //价格很小的情况下，比如0.01，舍入误差会让卖家拿不到钱
            if (SellerPercent > 0)
            {
                trade.BuyerAmount = Math.Round(trade.TotalSettlementAmount * (1m - PlatformPercent - SellerPercent), 2);
                trade.TotalSellerAmount = trade.TotalSettlementAmount - trade.PlatformAmount - trade.BuyerAmount;
            }
            else
            {
                trade.TotalSellerAmount = 0;
                trade.BuyerAmount = trade.TotalSettlementAmount - trade.PlatformAmount;
            }


            //order.tourist_amount = Math.Min(order.amount, ext.money.ceiling((1 - cost_percent) * order.amount));
            //var left = order.amount - order.tourist_amount;
            //order.platform_amount = ext.money.floor(left * config.instance.brokerage_percent);
            //order.guide_amount = left - order.platform_amount;
        }
        Task<TradeState> EvalSettlementState(DataModels.DataTrade trade, decimal PlatformPercent, decimal SellerPercent)
        {
            if (trade.State == TradeState.Created)
            {
                if (SellerPercent != 0)
                    throw new ArgumentException("买家未付款时无法退款");
                trade.BuyerAmount =
                trade.TotalSellerAmount =
                trade.PlatformAmount = 0;
                return Task.FromResult(TradeState.Closed);
            }
            CalcAmount(trade, PlatformPercent, SellerPercent);

            if (trade.BuyerAmount > 0)
            {
                if (trade.TotalSellerAmount > 0)
                    return Task.FromResult(TradeState.WaitSettlement);
                else
                    return Task.FromResult(TradeState.WaitBuyerSettlement);
            }
            else if (trade.TotalSellerAmount > 0)
                return Task.FromResult(TradeState.WaitSellerSettlement);
            else
            {
                return Task.FromResult(TradeState.Closed);
            }
        }
        public async Task BuyerAbort(long tradeId, string reason)
        {
            await Process(
            tradeId,
            TradeActionType.BuyerAbort,
            new[] { TradeState.Established },
            async (trade, time) =>
            {
                trade.EndType = TradeEndType.BuyerAborted;
                trade.EndReason = reason;
                trade.EndTime = time;
                var new_state = await EvalSettlementState(trade, 0, 1);

                return OK(new_state, $"买家终止订单");
            });
        }

        public async Task BuyerCancel(long tradeId, string reason, bool expired)
        {
            var re = await Process(
                    tradeId,
                    TradeActionType.BuyerCancel,
                    new[] { TradeState.Created, TradeState.WaitSellerConfirm },
                    async (trade, time) =>
                    {
                        trade.EndType =
                            expired ? TradeEndType.BuyerConfirmExpired :
                            trade.State == TradeState.WaitSellerConfirm ? TradeEndType.BuyerCancelledAfterConfirm :
                            TradeEndType.BuyerCancelledBeforeConfirm;

                        trade.EndReason = reason;

                        var new_state = await EvalSettlementState(trade, 0, 0);
                        return OK(new_state, trade.BuyerAmount > 0 ? "买家取消订单,等待退款。" : "买家取消订单");
                    });

            //var event_id =
            //	expired ? sys_events.sys_order_tourist_confirm_expired :
            //	re.state == order_state.closed ? sys_events.sys_order_tourist_cancelled_before_pay :
            //	sys_events.sys_order_tourist_cancelled_after_pay;
            //await order_event_service.raise_event(order_id, event_id);

            //return re;
        }

        public async Task BuyerComplete(long tradeId, bool expired)
        {
            await Process(
                tradeId,
                TradeActionType.BuyerComplete,
                new[] { TradeState.WaitBuyerComplete },
                async (trade, time) =>
                {
                    var new_state = await EvalSettlementState(trade, 0, 1);
                    trade.EndTime = time;
                    trade.EndType = TradeEndType.Completed;
                    return OK(
                        new_state,
                        expired ? $"买家评价超时" : $"买家已完成订单"
                        );
                });
        }

        public async Task BuyerConfirm(long tradeId, string paymentRecordId)
        {
            await Process(
                tradeId,
                TradeActionType.BuyerConfirm,
                new[] { TradeState.Created },
                (trade, time) =>
                {
                    //await payment_success(payment_log_id);

                    //卖家确认超时
                    //order.expires = time.AddHours(config.instance.order_timer_guide_confirm_expire);

                    return Task.FromResult(OK(TradeState.WaitSellerConfirm, "买家支付成功,等待确认"));
                });

        }

        public async Task BuyerSettlementCompleted(long tradeId, string paymentRecordId)
        {
            await Process(
                tradeId,
                TradeActionType.BuyerSettlementCompleted,
                new[] { TradeState.WaitSettlement, TradeState.WaitBuyerSettlement },
                (trade, time) =>
                {
                    //await payment_success(payment_log_id);

                    var name = trade.State == TradeState.WaitSettlement ? "买家已结算，等待卖家结算" : "买家已结算,订单完成";
                    return Task.FromResult(OK(
                        trade.State == TradeState.WaitSettlement ? TradeState.WaitSellerSettlement : TradeState.Closed,
                        name
                        ));
                });
        }

        public async Task SellerAbort(long tradeId, string reason)
        {
            await Process(
                tradeId,
                TradeActionType.SellerAbort,
                new[] { TradeState.Established },
                async (trade, time) =>
                {
                    trade.EndType = TradeEndType.BuyerAborted;
                    trade.EndReason = reason;
                    trade.EndTime = time;

                    var new_state = await EvalSettlementState(trade, 0, 0);

                    var desc = "";
                    if (trade.BuyerAmount > 0)
                        desc += $",退还买家{trade.BuyerAmount}元";
                    if (trade.TotalSellerAmount > 0)
                        desc += $",卖家保留{trade.TotalSellerAmount}元";
                    if (trade.PlatformAmount > 0)
                        desc += $",平台保留{trade.PlatformAmount}元";

                    return OK(new_state, $"卖家终止订单{desc}");
                });
        }

        public async Task SellerCancel(long tradeId, string reason, bool expired)
        {
            await Process(
                tradeId,
                TradeActionType.SellerCancel,
                new[] { TradeState.Created, TradeState.WaitSellerConfirm },
                async (trade, time) =>
                {
                    trade.EndType =
                        expired ? TradeEndType.BuyerConfirmExpired :
                        trade.State == TradeState.WaitSellerConfirm ? TradeEndType.BuyerCancelledAfterConfirm :
                        TradeEndType.BuyerCancelledBeforeConfirm;

                    trade.EndReason = reason;

                    var new_state = await EvalSettlementState(trade, 0, 0);
                    return OK(new_state, trade.BuyerAmount > 0 ? "卖家取消订单,等待退款。" : "买家取消订单");
                });

        }

        public async Task SellerComplete(long tradeId)
        {
            await Process(
                tradeId,
                TradeActionType.SellerComplete,
                new[] { TradeState.Established },
                (trade, time) =>
                {

                    return Task.FromResult(new Result
                    {
                        NewState = TradeState.WaitBuyerComplete,
                        EventName = $"卖家完成订单",//，给买家评分为:{Math.Round(score, 1)}",
                                              //timers = timers.ToArray()
                    });
                });
        }

        public async Task SellerConfirm(long tradeId)
        {
            await Process(
                tradeId,
                TradeActionType.SellerConfirm,
                new[] { TradeState.WaitSellerConfirm },
                (trade, time) =>
                {
                    //trade.begintime = time;

                    return Task.FromResult(new Result
                    {
                        NewState = TradeState.Established,
                        EventName = "卖家已接受预定"
                    });
                });
        }

        public async Task SellerSettlementCompleted(long tradeId, string paymentRecordId)
        {
            await Process(
               tradeId,
               TradeActionType.SellerSettlementCompleted,
               new[] { TradeState.WaitSettlement, TradeState.WaitSellerSettlement },
               (trade, time) =>
               {
                   //await payment_success(payment_log_id);

                   var name = trade.State == TradeState.WaitSettlement ? "卖家已结算，等待买家结算" : "卖家已结算,订单完成";
                   return Task.FromResult(OK(
                       trade.State == TradeState.WaitSettlement ?
                           TradeState.WaitSellerSettlement :
                           TradeState.Closed,
                           name
                           ));
               });
        }

    }
}
