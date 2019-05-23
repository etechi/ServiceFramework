using System;
using System.Linq;
using System.Threading.Tasks;
using SF.Sys;
using SF.Sys.Entities;
using SF.Sys.Data;
using SF.Biz.Trades.DataModels;
using System.Collections.Generic;
using SF.Sys.Services;
using System.Collections.Concurrent;
using SF.Sys.Reflection;
using SF.Sys.Linq;
using SF.Sys.Reminders;
using SF.Biz.Trades.StateProviders;
using SF.Common.Addresses.Management;

namespace SF.Biz.Trades.Managements
{


    public class TradeSetting {
        
    }

    public class TradeManager :
        AutoModifiableEntityManager<ObjectKey<long>, TradeInternal, TradeInternal, TradeQueryArguments, TradeInternal, DataModels.DataTrade>,
        ITradeManager
    {
        ITradeSyncQueue SyncQueue { get; }
        TradeSetting Setting { get; }
        
        public TradeManager(IEntityServiceContext ServiceContext, ITradeSyncQueue SyncQueue ,TradeSetting Setting) : base(ServiceContext)
        {
            this.Setting = Setting;
            this.SyncQueue = SyncQueue;
        }
                     


        void UpdateSettlementAmount(DataModels.DataTrade model,TradeInternal editable,Dictionary<long,TradeItemInternal> itemDict = null)
        {
            if (itemDict == null)
                itemDict = editable.Items.ToDictionary(ti => ti.Id);

            model.TotalSettlementAmount = 0;

            int itemCount = 0;
            foreach (var mi in model.Items)
            {
                var ei = itemDict[mi.Id];
                mi.SettlementAmount = ei.SettlementAmount;
                model.TotalSettlementAmount += mi.SettlementAmount;
                itemCount++;
            }
            model.TotalSettlementLeftAmount = model.TotalSettlementAmount;

            var left = model.TotalSettlementAmount;
            var i = 0;
            foreach (var mi in model.Items)
            {
                i++;
                if (i == itemCount)
                {
                    mi.ApportionAmount = left;
                    mi.ApportionPrice = Math.Floor(mi.ApportionAmount * 100m / mi.Quantity) * 0.01m;
                }
                else
                {
                    mi.ApportionPrice = Math.Floor(mi.Amount * model.TotalSettlementAmount * 100m / model.TotalAmount / mi.Quantity) * 0.01m;
                    mi.ApportionAmount = mi.ApportionPrice * mi.Quantity;
                    left -= mi.ApportionAmount;
                }
            }
        }


        async Task FillTradeItemFields(TradeInternal editable)
        {
            var eir = ServiceContext.ServiceProvider.Resolve<ITradableItemResolver>();
            editable.Amount = 0;
            var addressRequired = false;
            foreach (var g in editable.Items.GroupBy(i => i.ProductId.LeftBefore('-')))
            {
                if (g.Key.IsNullOrEmpty())
                    throw new PublicArgumentException("部分项目未提供商品类型");

                var tradables = (await eir.Resolve(g.Select(i => i.ProductId).ToArray())).ToDictionary(i=>i.Id);
                foreach (var i in g)
                {   
                    if (!tradables.TryGetValue(i.ProductId, out var tradable))
                        throw new PublicArgumentException($"找不到商品:{i.ProductId}");

                    if (i.LogicState != EntityLogicState.Enabled)
                        throw new PublicArgumentException($"{tradable.Title}已下架");

                    i.Name = tradable.Name;
                    i.Title = tradable.Title;
                    i.Price = tradable.Price;
                    i.Image = tradable.Image;
                    i.Amount = i.Quantity * i.Price;
                    i.DeliveryProvider = tradable.DeliveryProvider;
                    i.SellerId = tradable.SellerId;
                    i.BuyerId = editable.BuyerId;

                    editable.Amount += i.Amount;

                    if (!addressRequired)
                    {
                        var dp = ServiceContext.ServiceProvider.Resolve<ITradeDeliveryProvider>(i.DeliveryProvider);
                        if (dp.DeliveryAddressRequired)
                            addressRequired = true;
                    }
                }
            }
            if(addressRequired)
            {
                editable.DeliveryAddressRequired = addressRequired;
            }
        }
        async Task ResolveDiscounts(TradeInternal trade)
        {
            var ids=trade.Items.Select(i => i.DiscountEntityIdent).WithLast(trade.DiscountEntityId).Where(i => i.HasContent()).ToArray();
            var discounts=await ServiceContext.ServiceProvider.Resolve<IEntityReferenceResolver>().Resolve(null, ids);
            var discountDict=discounts.ToDictionary(d => d.Id, d => d.Cast<IDiscountItem>());

            trade.AmountAfterDiscount = 0;
            trade.DiscountAmount = 0;
            foreach(var item in trade.Items)
            {
                item.PriceAfterDiscount = item.Price;
                item.AmountAfterDiscount = item.Amount;
                item.DiscountAmount = 0;
                item.DiscountDesc = null;

                if (item.DiscountEntityIdent.HasContent())
                {
                    if (!discountDict.TryGetValue(item.DiscountEntityIdent, out var discount))
                        throw new PublicArgumentException("找不到优惠券:" + item.DiscountEntityIdent);

                    await discount.Apply(item, trade);

                    Ensure.Equal(item.AmountAfterDiscount - item.Amount, item.DiscountAmount, "则扣金额错误");
                    Ensure.Equal(item.AmountAfterDiscount, item.Quantity * item.PriceAfterDiscount, "则扣后小计错误");
                    Ensure.HasContent(item.DiscountDesc, "折扣说明");
                }

                item.SettlementAmount = item.AmountAfterDiscount;
                trade.DiscountAmount += item.DiscountAmount;
                trade.AmountAfterDiscount += item.AmountAfterDiscount;
            }
            trade.TotalSettlementAmount = trade.AmountAfterDiscount;


            if (trade.DiscountEntityId.HasContent())
            {
                if (!discountDict.TryGetValue(trade.DiscountEntityId, out var discount))
                    throw new PublicArgumentException("找不到优惠券:" + trade.DiscountEntityId);
                await discount.Apply(trade);
                Ensure.Equal(trade.AmountAfterDiscount - trade.Amount, trade.DiscountAmount, "则扣金额错误");
                Ensure.HasContent(trade.DiscountDesc, "折扣说明");
            }
        }

        async Task FillDefaultDestAddress(DataTrade model)
        {

            if (!model.DeliveryAddressRequired || model.DeliveryAddressId.HasValue)
                return;
            model.DeliveryAddressId = (await ServiceContext.ServiceProvider.Resolve<IUserAddressManager>()
                .QueryIdentsAsync(new UserAddressQueryArguments
                {
                    OwnerId = model.BuyerId,
                    IsDefaultAddress = true
                }))
                .Items
                .FirstOrDefault()?.Id;

        }
        void InitModel(DataTrade model, TradeInternal editable, Dictionary<long, TradeItemInternal> itemDict)
        {

            if (editable.BizRoot == null)
                editable.BizRoot = editable.BizParent = new TrackIdent("交易", "Trade", model.Id);
            model.BizRoot = editable.BizRoot;
            model.BizParent = editable.BizParent;

            model.Name = editable.Name;
            model.Title = editable.Title;
            model.Image = editable.Image;

            model.TotalAmount = 0;
            model.TotalSettlementAmount = 0;
            model.TotalQuantity = 0;
            model.TotalAmountAfterDiscount = 0;
            model.TotalDiscountAmount = 0;
            model.State = TradeState.BuyerConfirm;
            model.DeliveryAddressRequired = editable.DeliveryAddressRequired;

            foreach (var mi in model.Items)
            {
                var ei = itemDict[mi.Id];

                mi.Name = ei.Name;
                mi.Title = ei.Title;
                mi.BuyerId = ei.BuyerId;
                mi.SellerId = ei.SellerId;
                mi.Price = ei.Price;
                mi.DeliveryProvider = ei.DeliveryProvider;
                mi.Amount = ei.Amount = ei.Quantity * ei.Price;

                Ensure.Equal(ei.Quantity * ei.PriceAfterDiscount, ei.AmountAfterDiscount, "折扣后小计错误");
                Ensure.Equal(ei.DiscountAmount, ei.Amount - ei.AmountAfterDiscount, "则扣金额错误");

                mi.PriceAfterDiscount = ei.PriceAfterDiscount;
                mi.AmountAfterDiscount = ei.AmountAfterDiscount;
                mi.DiscountAmount = ei.DiscountAmount;

                model.TotalAmount += mi.Amount;
                model.TotalDiscountAmount += mi.DiscountAmount;
                model.TotalAmountAfterDiscount += mi.AmountAfterDiscount;
                model.TotalQuantity += mi.Quantity;
            }
            if (!model.Name.HasContent())
            {
                var item = model.Items.OrderByDescending(i => i.SettlementAmount).First();
                model.Name = item.Name;
                model.Title = item.Title;
                var itemCount = model.Items.Count();
                if (itemCount > 1)
                {
                    var profix = $"(等{itemCount}件";
                    model.Name += profix;
                    model.Title += profix;
                }
            }
        }
        protected override async Task OnNewModel(IModifyContext ctx)
        {
            var editable = ctx.Editable;
            if ((editable.BizRoot == null) != (editable.BizParent==null))
                throw new ArgumentException("必须同时指定根业务和父业务");
            await base.OnNewModel(ctx);
            
        }
        protected override async Task OnUpdateModel(IModifyContext ctx)
        {
            var model = ctx.Model;
            if (model.State != TradeState.BuyerConfirm || model.StateExecStartTime.HasValue)
                throw new PublicInvalidOperationException("订单不支持修改");


            var editable = ctx.Editable;

            await base.OnUpdateModel(ctx);

            if (ctx.Action == ModifyAction.Create)
                foreach (var p in editable.Items.Zip(ctx.Model.Items, (ii, mi) => (ii, mi)))
                    p.ii.Id = p.mi.Id;

            var itemDict = editable.Items.ToDictionary(i => i.Id);

            if (ctx.Action == ModifyAction.Create)
            {
                //填充商品信息
                await FillTradeItemFields(editable);

                //常规批价
                await ResolveDiscounts(editable);

                InitModel(model, editable, itemDict);

                await FillDefaultDestAddress(model);
            }

            //计算结算价格
            UpdateSettlementAmount(model, editable, itemDict);
            
            //验证交易信息
            foreach (var tv in ServiceContext.ServiceProvider.Resolve<IEnumerable<ITradeValidator>>())
                await tv.Validate(editable);
            
            if(ctx.Action==ModifyAction.Create)
                await WithTrade(model,null, Advance);

        }



        async Task<TradeExecResult> WithTrade(
            DataTrade trade,
            IArgumentWithExpires Argument,
            Func<DataModels.DataTrade, IArgumentWithExpires, Task<TradeExecResult>> callback
            )
        {
            TradeExecResult result = null;

            var bizRoot = TrackIdent.Parse(trade.BizRoot);
            if (trade.ReminderSetuped && !(Argument is TradeStateRemindArgument))
            {
                //如果提醒器已注册且调用不是来自提醒器
                await ServiceContext.ServiceProvider.Resolve<IRemindService>().Remind(
                    bizRoot,
                    new Func<IArgumentWithExpires, Task<TradeExecResult>>(async (arg) =>
                    {
                        if (Argument == null)
                            Argument = arg;
                        else if (!Argument.Expires.HasValue)
                            Argument.Expires = arg.Expires;
                        result = await callback(trade, Argument);
                        if (!result.Expires.HasValue)
                            trade.ReminderSetuped = false;
                        return result;
                    }));
            }
            else
            {

                result = await callback(trade, Argument);
                //如果交易为顶层业务，需要注册提醒器
                if (result.Expires.HasValue )
                {
                    if(bizRoot.IdentType == "Trade")
                        await ServiceContext.ServiceProvider.Resolve<IRemindService>().Setup(new RemindSetupArgument
                        {
                            BizSource = bizRoot,
                            Name = trade.Name,
                            RemindableName = typeof(TradeRemindable).FullName,
                            RemindTime = result.Expires.Value,
                            UserId = trade.SellerId
                        });
                    trade.ReminderSetuped = true;
                }
                else
                    trade.ReminderSetuped = false;
            }
            return result;
        }

        Task<TradeExecResult> WithTrade(
            long TradeId,
            TradeState? ExpectState,
            IArgumentWithExpires Argument,
            Func<DataModels.DataTrade, IArgumentWithExpires, Task<TradeExecResult>> callback
            )
        {
            return SyncQueue.Queue(
                TradeId,
                () => DataScope.Use("处理订单", async ctx =>
                {
                    var trade = await ctx.Queryable<DataModels.DataTrade>()
                        .Where(t => t.Id.Equals(TradeId))
                        .Include(t => t.Items)
                        .SingleOrDefaultAsync();

                    if (trade == null)
                        throw new TradeException("订单不存在");

                    if (trade.LogicState != EntityLogicState.Enabled)
                        throw new PublicDeniedException("订单不能操作");
                    if (ExpectState.HasValue && trade.State != ExpectState.Value)
                        throw new PublicInvalidOperationException($"订单{trade.Id}操作状态错误，期望状态{ExpectState},当前状态{trade.State}");

                    var result = await WithTrade(trade, Argument, callback);

                    trade.SyncItemsState(ctx);
                    ctx.Update(trade);
                    await ctx.SaveChangesAsync();
                    return result;
                }));
        }


        async Task<TradeExecResult> Step(DataModels.DataTrade trade, ITradeStateProvider provider, IArgumentWithExpires arg)
        {
            //超时
            var expires = arg?.Expires;
            var expired = (arg is TradeStateRemindArgument) && expires.HasValue && expires.Value < Now;
            TradeExecResult result;
            if (expired)
            {
                if (trade.StateExecStartTime.HasValue)
                    result = await provider.ExecutingExpired(trade);
                else
                    result = await provider.AdvanceExpired(trade);
            }
            else
            {
                if (trade.StateExecStartTime.HasValue && (!provider.Restartable || arg == null || arg is TradeStateRemindArgument))
                {
                    if (!provider.Restartable && arg != null && !(arg is TradeStateRemindArgument))
                        throw new InvalidOperationException($"订单状态{trade.State}正在执行,不接受类型为{arg.GetType()}的参数");

                    result = await provider.UpdateStatus(trade, expires);
                }
                else
                {
                    result = await provider.Advance(trade, arg);
                }
            }
            switch (result.Status)
            {
                case TradeExecStatus.ArgumentRequired:
                    var timeout = provider.AdvanceWaitTimeout;
                    if (timeout.HasValue && !trade.ReminderSetuped)
                        result.Expires = Now.Add(timeout.Value);
                    break;
                case TradeExecStatus.Executing:
                    if (!trade.StateExecStartTime.HasValue)
                        trade.StateExecStartTime = Now;
                    break;
                case TradeExecStatus.IsCompleted:
                    trade.StateExecStartTime = null;
                    trade.State = result.NextState;
                    trade.LastStateTime = Now;
                    trade.EndType = result.EndType;
                    trade.EndReason = result.EndReason;
                    break;
            }
            return result;
        }
        public ITradeStateProvider GetStateProvider(TradeState State)
        {
            return ServiceContext.ServiceProvider.Resolve<ITradeStateProvider>(State.ToString());
        }
        async Task<TradeExecResult> Advance(DataTrade trade,IArgumentWithExpires arg)
        {
            if (trade.State == TradeState.Closed)
                return new TradeExecResult
                {
                    Status = TradeExecStatus.IsCompleted
                };

            var firstRun = true;
            TradeExecResult result = null;
            for (; ; )
            {
                var provider = GetStateProvider(trade.State);
                if (provider == null)
                    throw new PublicInvalidOperationException("不支持此状态:" + trade.State);
                if (firstRun)
                    firstRun = false;
                else if (provider.AdvanceWaitTimeout.HasValue)
                {
                    result.Expires = Now.Add(provider.AdvanceWaitTimeout.Value);
                    return result;
                }

                result = await Step(trade, provider, arg);

                if (result.Status == TradeExecStatus.ArgumentRequired ||
                    result.Status == TradeExecStatus.Executing ||
                    trade.State == TradeState.Closed
                    )
                    return result;

                arg = null;
            }
        }
        public Task<TradeExecResult> Advance(long TradeId, TradeState? ExpectState, IArgumentWithExpires Argument)
        {
            return WithTrade(TradeId, ExpectState, Argument, Advance);
        }

        //protected class Result
        //{
        //    public TradeState NewState { get; set; }
        //    public DateTime? NextTimeout { get; set; }
        //    public string EventName { get; set; }
        //}
        //protected virtual Task<DataModels.DataTrade> Process(
        //    long TradeId,
        //    TradeActionType ActionType,
        //    TradeState[] StateRequired,
        //    Func<DataModels.DataTrade, DateTime, Task<Result>> action
        //    )
        //{
        //    return SyncQueue.Queue(
        //        TradeId,
        //        () =>DataScope.Use("处理订单", async ctx =>
        //        {
        //            var time = Now;
        //            var trade = await ctx.Queryable<DataModels.DataTrade>()
        //                .Where(t => t.Id.Equals(TradeId))
        //                .Include(t => t.Items)
        //                .SingleOrDefaultAsync();

        //            if (trade == null)
        //                throw new TradeException("订单不存在");
        //            if (!StateRequired.Contains(trade.State))
        //                throw new TradeException($"订单状态错误，当前状态:{trade.State} 当前操作:{ActionType}");

        //            await ServiceContext.ServiceProvider.Resolve<IRemindService>().Remind(
        //                "交易",
        //                "Trade", 
        //                TradeId, 
        //                new Func<Task<DateTime?>>(async () => { 
        //                    var org_state = trade.State;
        //                    var org_end_type = trade.EndType;

        //                    //清除超时时间
        //                    trade.StateExpires = null;
        //                    var re = await action(trade, time);
        //                    trade.State = re.NewState;
        //                    trade.LastStateTime = time;
        //                    trade.StateExpires = re.NextTimeout;

        //                    return re.NextTimeout;
        //                })
        //               );
        //            trade.SyncItemsState(ctx);
        //            ctx.Update(trade);
        //            await ctx.SaveChangesAsync();
        //            return trade;
        //        })
        //        );

        //}
        //Result OK(TradeState state, string eventName)
        //{
        //    return new Result { EventName = eventName, NewState = state };
        //}

        //void CalcAmount(DataModels.DataTrade trade, decimal PlatformPercent, decimal SellerPercent)
        //{
        //    var buyer_amount = trade.TotalSettlementAmount;
        //    if (PlatformPercent + SellerPercent > 1)
        //        throw new TradeException("金额比例错误");

        //    trade.PlatformAmount = Math.Round(trade.TotalSettlementAmount * PlatformPercent, 2);

        //    //价格很小的情况下，比如0.01，舍入误差会让卖家拿不到钱
        //    if (SellerPercent > 0)
        //    {
        //        trade.BuyerAmount = Math.Round(trade.TotalSettlementAmount * (1m - PlatformPercent - SellerPercent), 2);
        //        trade.TotalSellerAmount = trade.TotalSettlementAmount - trade.PlatformAmount - trade.BuyerAmount;
        //    }
        //    else
        //    {
        //        trade.TotalSellerAmount = 0;
        //        trade.BuyerAmount = trade.TotalSettlementAmount - trade.PlatformAmount;
        //    }


        //    //order.tourist_amount = Math.Min(order.amount, ext.money.ceiling((1 - cost_percent) * order.amount));
        //    //var left = order.amount - order.tourist_amount;
        //    //order.platform_amount = ext.money.floor(left * config.instance.brokerage_percent);
        //    //order.guide_amount = left - order.platform_amount;
        //}
        //Task<TradeState> EvalSettlementState(DataModels.DataTrade trade, decimal PlatformPercent, decimal SellerPercent)
        //{
        //    if (trade.State == TradeState.WaitBuyerStartConfirm)
        //    {
        //        if (SellerPercent != 0)
        //            throw new ArgumentException("买家未付款时无法退款");
        //        trade.BuyerAmount =
        //        trade.TotalSellerAmount =
        //        trade.PlatformAmount = 0;
        //        return Task.FromResult(TradeState.Closed);
        //    }
        //    CalcAmount(trade, PlatformPercent, SellerPercent);

        //    if (trade.BuyerAmount > 0)
        //    {
        //        if (trade.TotalSellerAmount > 0)
        //            return Task.FromResult(TradeState.WaitSettlement);
        //        else
        //            return Task.FromResult(TradeState.WaitBuyerSettlement);
        //    }
        //    else if (trade.TotalSellerAmount > 0)
        //        return Task.FromResult(TradeState.WaitSellerSettlement);
        //    else
        //    {
        //        return Task.FromResult(TradeState.Closed);
        //    }
        //}
        //public async Task BuyerAbort(long tradeId, string reason)
        //{
        //    await Process(
        //    tradeId,
        //    TradeActionType.BuyerAbort,
        //    new[] { TradeState.Established },
        //    async (trade, time) =>
        //    {
        //        trade.EndType = TradeEndType.BuyerAborted;
        //        trade.EndReason = reason;
        //        trade.EndTime = time;
        //        var new_state = await EvalSettlementState(trade, 0, 1);

        //        return OK(new_state, $"买家终止订单");
        //    });
        //}

        //public async Task BuyerCancel(long tradeId, string reason, bool expired)
        //{
        //    var re = await Process(
        //            tradeId,
        //            TradeActionType.BuyerCancel,
        //            new[] { TradeState.WaitBuyerStartConfirm, TradeState.WaitSellerConfirm },
        //            async (trade, time) =>
        //            {
        //                trade.EndType =
        //                    expired ? TradeEndType.BuyerConfirmExpired :
        //                    trade.State == TradeState.WaitSellerConfirm ? TradeEndType.BuyerCancelledAfterConfirm :
        //                    TradeEndType.BuyerCancelledBeforeConfirm;

        //                trade.EndReason = reason;

        //                var new_state = await EvalSettlementState(trade, 0, 0);
        //                return OK(new_state, trade.BuyerAmount > 0 ? "买家取消订单,等待退款。" : "买家取消订单");
        //            });

        //    //var event_id =
        //    //	expired ? sys_events.sys_order_tourist_confirm_expired :
        //    //	re.state == order_state.closed ? sys_events.sys_order_tourist_cancelled_before_pay :
        //    //	sys_events.sys_order_tourist_cancelled_after_pay;
        //    //await order_event_service.raise_event(order_id, event_id);

        //    //return re;
        //}

        //public async Task BuyerComplete(long tradeId, bool expired)
        //{
        //    await Process(
        //        tradeId,
        //        TradeActionType.BuyerComplete,
        //        new[] { TradeState.WaitBuyerComplete },
        //        async (trade, time) =>
        //        {
        //            var new_state = await EvalSettlementState(trade, 0, 1);
        //            trade.EndTime = time;
        //            trade.EndType = TradeEndType.Completed;
        //            return OK(
        //                new_state,
        //                expired ? $"买家评价超时" : $"买家已完成订单"
        //                );
        //        });
        //}

        //public async Task BuyerConfirm(long tradeId, string paymentRecordId)
        //{
        //    await Process(
        //        tradeId,
        //        TradeActionType.BuyerConfirm,
        //        new[] { TradeState.WaitBuyerStartConfirm },
        //        (trade, time) =>
        //        {
        //            //await payment_success(payment_log_id);

        //            //卖家确认超时
        //            //order.expires = time.AddHours(config.instance.order_timer_guide_confirm_expire);

        //            return Task.FromResult(OK(TradeState.WaitSellerConfirm, "买家支付成功,等待确认"));
        //        });

        //}

        //public async Task BuyerSettlementCompleted(long tradeId, string paymentRecordId)
        //{
        //    await Process(
        //        tradeId,
        //        TradeActionType.BuyerSettlementCompleted,
        //        new[] { TradeState.WaitSettlement, TradeState.WaitBuyerSettlement },
        //        (trade, time) =>
        //        {
        //            //await payment_success(payment_log_id);

        //            var name = trade.State == TradeState.WaitSettlement ? "买家已结算，等待卖家结算" : "买家已结算,订单完成";
        //            return Task.FromResult(OK(
        //                trade.State == TradeState.WaitSettlement ? TradeState.WaitSellerSettlement : TradeState.Closed,
        //                name
        //                ));
        //        });
        //}

        //public async Task SellerAbort(long tradeId, string reason)
        //{
        //    await Process(
        //        tradeId,
        //        TradeActionType.SellerAbort,
        //        new[] { TradeState.Established },
        //        async (trade, time) =>
        //        {
        //            trade.EndType = TradeEndType.BuyerAborted;
        //            trade.EndReason = reason;
        //            trade.EndTime = time;

        //            var new_state = await EvalSettlementState(trade, 0, 0);

        //            var desc = "";
        //            if (trade.BuyerAmount > 0)
        //                desc += $",退还买家{trade.BuyerAmount}元";
        //            if (trade.TotalSellerAmount > 0)
        //                desc += $",卖家保留{trade.TotalSellerAmount}元";
        //            if (trade.PlatformAmount > 0)
        //                desc += $",平台保留{trade.PlatformAmount}元";

        //            return OK(new_state, $"卖家终止订单{desc}");
        //        });
        //}

        //public async Task SellerCancel(long tradeId, string reason, bool expired)
        //{
        //    await Process(
        //        tradeId,
        //        TradeActionType.SellerCancel,
        //        new[] { TradeState.WaitBuyerStartConfirm, TradeState.WaitSellerConfirm },
        //        async (trade, time) =>
        //        {
        //            trade.EndType =
        //                expired ? TradeEndType.BuyerConfirmExpired :
        //                trade.State == TradeState.WaitSellerConfirm ? TradeEndType.BuyerCancelledAfterConfirm :
        //                TradeEndType.BuyerCancelledBeforeConfirm;

        //            trade.EndReason = reason;

        //            var new_state = await EvalSettlementState(trade, 0, 0);
        //            return OK(new_state, trade.BuyerAmount > 0 ? "卖家取消订单,等待退款。" : "买家取消订单");
        //        });

        //}

        //public async Task SellerComplete(long tradeId)
        //{
        //    await Process(
        //        tradeId,
        //        TradeActionType.SellerComplete,
        //        new[] { TradeState.Established },
        //        (trade, time) =>
        //        {

        //            return Task.FromResult(new Result
        //            {
        //                NewState = TradeState.WaitBuyerComplete,
        //                EventName = $"卖家完成订单",//，给买家评分为:{Math.Round(score, 1)}",
        //                                      //timers = timers.ToArray()
        //            });
        //        });
        //}

        //public async Task SellerConfirm(long tradeId)
        //{
        //    await Process(
        //        tradeId,
        //        TradeActionType.SellerConfirm,
        //        new[] { TradeState.WaitSellerConfirm },
        //        (trade, time) =>
        //        {
        //            //trade.begintime = time;

        //            return Task.FromResult(new Result
        //            {
        //                NewState = TradeState.Established,
        //                EventName = "卖家已接受预定"
        //            });
        //        });
        //}

        //public async Task SellerSettlementCompleted(long tradeId, string paymentRecordId)
        //{
        //    await Process(
        //       tradeId,
        //       TradeActionType.SellerSettlementCompleted,
        //       new[] { TradeState.WaitSettlement, TradeState.WaitSellerSettlement },
        //       (trade, time) =>
        //       {
        //           //await payment_success(payment_log_id);

        //           var name = trade.State == TradeState.WaitSettlement ? "卖家已结算，等待买家结算" : "卖家已结算,订单完成";
        //           return Task.FromResult(OK(
        //               trade.State == TradeState.WaitSettlement ?
        //                   TradeState.WaitSellerSettlement :
        //                   TradeState.Closed,
        //                   name
        //                   ));
        //       });
        //}

        //Task ITradeController.SellerCancel(long tradeId, string reason, bool expired)
        //{
        //    throw new NotImplementedException();
        //}

        //Task ITradeController.SellerConfirm(long tradeId)
        //{
        //    throw new NotImplementedException();
        //}

        //Task ITradeController.SellerAbort(long tradeId, string reason)
        //{
        //    throw new NotImplementedException();
        //}

        //Task ITradeController.SellerComplete(long tradeId)
        //{
        //    throw new NotImplementedException();
        //}

        //Task ITradeController.SellerSettlementCompleted(long tradeId, string paymentRecordId)
        //{
        //    throw new NotImplementedException();
        //}

        //Task ITradeController.BuyerCancel(long tradeId, string reason, bool expired)
        //{
        //    throw new NotImplementedException();
        //}

        //Task ITradeController.BuyerConfirm(long tradeId, string paymentRecordId)
        //{
        //    throw new NotImplementedException();
        //}

        //Task ITradeController.BuyerAbort(long tradeId, string reason)
        //{
        //    throw new NotImplementedException();
        //}

        //Task ITradeController.BuyerComplete(long tradeId, bool expired)
        //{
        //    throw new NotImplementedException();
        //}

        //Task ITradeController.BuyerSettlementCompleted(long tradeId, string paymentRecordId)
        //{
        //    throw new NotImplementedException();
        //}

        //Task<DateTime?> ITradeController.Advance(long tradeId,bool Expired)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
