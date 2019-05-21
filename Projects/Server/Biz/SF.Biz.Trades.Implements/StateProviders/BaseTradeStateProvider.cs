using SF.Biz.Trades.DataModels;
using SF.Biz.Trades.Managements;
using SF.Sys.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SF.Biz.Trades.StateProviders
{
    public class NoArgumentRequired : IArgumentWithExpires
    {
        public DateTime? Expires { get; set; }
    }

    public abstract class BaseTradeStateProvider<TArgument> : ITradeStateProvider 
        where TArgument : class,IArgumentWithExpires,new()
    {

        public abstract string OpName { get; }
        public virtual bool Restartable => false;
        public virtual TimeSpan? AdvanceWaitTimeout => null;

        static bool TryGetArgument(IArgumentWithExpires Argument,out TArgument arg,out TradeExecResult result)
        {
            arg = null;
            result = null;
            if (typeof(TArgument) == typeof(NoArgumentRequired))
            {
                if (Argument != null && !(Argument is TradeStateRemindArgument))
                    throw new ArgumentException($"此状态不需要提供参数");
                if (Argument is TradeStateRemindArgument tsra)
                    arg = new TArgument
                    {
                        Expires = tsra.Expires
                    };
            }
            else
            {
                if (Argument == null)
                {
                    result = new TradeExecResult
                    {
                        Status = TradeExecStatus.ArgumentRequired
                    };
                    return false;
                }
                arg = Argument as TArgument;
                if (arg == null)
                    throw new ArgumentException($"参数类型错误: 期望类型:{typeof(TArgument)} 实际类型:{Argument.GetType()}");

            }
            return true;

        }
        public virtual Task<TradeExecResult> Advance(DataTrade trade, IArgumentWithExpires Argument)
        {
            if (!TryGetArgument(Argument, out var arg, out var result))
                return Task.FromResult(result);
            return Advance(trade, arg);
        }
        protected abstract Task<TradeExecResult> Advance(DataTrade trade, TArgument Argument);
        public abstract Task<TradeExecResult> UpdateStatus(DataTrade trade, DateTime? Expires);


        public abstract Task<TradeExecResult> ExecutingExpired(DataTrade trade);
        public abstract Task<TradeExecResult> AdvanceExpired(DataTrade trade);

        

    }
}
