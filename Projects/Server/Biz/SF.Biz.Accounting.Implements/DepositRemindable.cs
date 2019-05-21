using SF.Biz.Payments;
using SF.Sys.Reminders;
using SF.Sys.TimeServices;
using System;
using System.Threading.Tasks;

namespace SF.Biz.Accounting
{
    public class DepositRemindable : IRemindable
    {
        public IDepositService DepositService { get; }
        public ITimeService TimeService { get; }
        public IPaymentPlatformService PaymentPlatformService { get; }
        public DepositRemindable(IDepositService DepositService, ITimeService TimeService, IPaymentPlatformService PaymentPlatformService)
        {
            this.DepositService = DepositService;
            this.TimeService = TimeService;
            this.PaymentPlatformService = PaymentPlatformService;
        }
        public static DateTime GetEndTime(IPaymentPlatformService PaymentPlatformService,long PlatformId,DateTime StartTime)
        {
            return StartTime
                .Add(PaymentPlatformService.GetCollectRequestTimeout(PlatformId) ?? TimeSpan.FromDays(1))
                .AddMinutes(5);
        }
        public async Task Remind(IRemindContext Context)
        {
            var re=await DepositService.GetResult(Context.BizSource.Ident, true, false);
            //如果已完成,直接返回，结束提醒
            if (re.State != DepositState.Processing)
                return;

            var target = GetEndTime( PaymentPlatformService, re.PaymentPlatformId,re.Time);

            //如果还未到达结束时间，继续等待
            if (TimeService.Now < target)
                Context.NextRemindTime = target.AddMinutes(5);

            //超过结束时间未完成，之后每天确认一次
            else
                Context.NextRemindTime = target.AddDays(1);
        }
    }
}
