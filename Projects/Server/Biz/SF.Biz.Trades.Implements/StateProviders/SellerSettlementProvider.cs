using SF.Biz.Accounting;
using SF.Biz.Trades.DataModels;
using SF.Biz.Trades.Managements;
using SF.Sys;
using SF.Sys.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SF.Biz.Trades.StateProviders
{
    public class SellerSettlementProvider : BaseTradeStateProvider<NoArgumentRequired>
    {
        public override string OpName => "卖家结算";


        ISettlementManager SettlementManager { get; }
        Lazy<ISettlementRollbackManager> RollbackManager { get; }
        public SellerSettlementProvider(
            ISettlementManager SettlementManager, 
            Lazy<ISettlementRollbackManager> RollbackManager
            )
        {
            this.SettlementManager = SettlementManager;
            this.RollbackManager = RollbackManager;
        }

        TradeExecResult ProcessRollbackResult(DataTrade trade, SettlementRollbackStatus re)
        {
            switch (re.State)
            {
                case SettlementRollbackState.Processing:
                    return new TradeExecResult
                    {
                        Status = TradeExecStatus.Executing,
                        Expires = re.Expires
                    };
                case SettlementRollbackState.Failed:
                    throw new PublicInvalidOperationException("退款失败，请联系管理员处理:" + trade.Id);

                case SettlementRollbackState.Success:
                    trade.TotalSettlementRollbackAmount += trade.TotalSettlementLeftAmount;
                    trade.TotalSettlementLeftAmount = 0;

                    return new TradeExecResult
                    {
                        EndReason = trade.EndReason,
                        EndType = trade.EndType,
                        Status = TradeExecStatus.IsCompleted,
                        NextState = TradeState.Closed
                    };
                default:
                    throw new InvalidOperationException();
            }
        }
        async Task<TradeExecResult> Success(DataTrade trade)
        {
            await SettlementManager.Confirm(trade.SettlementRecordId.Value);
            return new TradeExecResult
            {
                EndType = TradeEndType.Completed,
                Status = TradeExecStatus.IsCompleted,
                NextState = TradeState.Closed
            };
        }
        public override Task<TradeExecResult> AdvanceExpired(DataTrade trade)
        {
            throw new NotImplementedException();
        }

        public override async Task<TradeExecResult> ExecutingExpired(DataTrade trade)
        {

            if (trade.EndType != TradeEndType.InProcessing)
            {
                var re = await RollbackManager.Value.UpdateAndQueryStatus(trade.Id);
                return ProcessRollbackResult(trade, re);

            }
            return await Success(trade);
        }

        
        public override async Task<TradeExecResult> UpdateStatus(DataTrade trade, DateTime? Expires)
        {
            if (trade.EndType != TradeEndType.InProcessing)
            {
                var re = await RollbackManager.Value.UpdateAndQueryStatus(trade.Id);
                return ProcessRollbackResult(trade, re);

            }
            else
                return await Success(trade);
        }

        protected override async Task<TradeExecResult> Advance(DataTrade trade, NoArgumentRequired Argument)
        {
            if (trade.EndType != TradeEndType.InProcessing)
            {
                var re = await RollbackManager.Value.Create(new SettlementRollbackCreateArgument
                {
                    Amount = trade.TotalSettlementLeftAmount,
                    BizParent = new TrackIdent("交易", "Trade", trade.Id),
                    BizRoot = trade.BizRoot,
                    Desc = trade.EndReason,
                    Name = trade.Name,
                    SettlementId = trade.SettlementRecordId.Value
                });
                return ProcessRollbackResult(trade, re);

            }
            else
                return await Success(trade);
        }
    }
}
