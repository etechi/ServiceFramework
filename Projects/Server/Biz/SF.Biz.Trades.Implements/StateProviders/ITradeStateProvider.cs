using SF.Biz.Trades.Managements;
using SF.Sys.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SF.Biz.Trades.StateProviders
{
  
    public interface ITradeStateProvider
    {
        bool Restartable { get; }
        TimeSpan? AdvanceWaitTimeout { get; }
        Task<TradeExecResult> Advance(DataModels.DataTrade trade, IArgumentWithExpires Argument);
        Task<TradeExecResult> UpdateStatus(DataModels.DataTrade trade, DateTime? Expires);
        Task<TradeExecResult> AdvanceExpired(DataModels.DataTrade trade);
        Task<TradeExecResult> ExecutingExpired(DataModels.DataTrade trade);

    }
}
