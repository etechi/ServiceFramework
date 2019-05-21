using SF.Sys.Reminders;
using SF.Sys.Threading;
using System;
using System.Threading.Tasks;

namespace SF.Biz.Trades.Managements
{

    public class TradeRemindable : IRemindable
    {
        ITradeManager TradeManager;
        public TradeRemindable(ITradeManager TradeManager)
        {
            this.TradeManager = TradeManager;
        }
        public async Task Remind(IRemindContext Context)
        {
            var arg = new TradeStateRemindArgument
            {
                Expires = Context.CurrentRemindTime
            };
            var result = Context.Argument == null ?
                await TradeManager.Advance(Context.BizSource.Ident,null, arg) :
                await ((Func<IArgumentWithExpires, Task<TradeExecResult>>)Context.Argument)(arg);

            if (result.Expires.HasValue)
                Context.NextRemindTime = result.Expires.Value;
        }
    }
}
