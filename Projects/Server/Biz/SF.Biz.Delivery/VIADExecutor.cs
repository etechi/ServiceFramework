using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SF.Sys.Data;
using SF.Sys.Linq;
using SF.Sys.Logging;
using SF.Sys.Services;
using SF.Sys.TimeServices;

namespace SF.Biz.Delivery
{
    public static class VIADExecutor
	{
		public static async Task<bool> Execute(
			IServiceProvider sp,
			VIADDelivery delivery
			)
		{
            return await sp.WithScope(async isp =>
            {
                var VIADDeliveryService = isp.Resolve<IVIADDeliveryService>();
                var VIADSupport = isp.Resolve<IVIADDeliverySupport>();
                var Time = isp.Resolve<ITimeService>().Now;
                var logger = isp.Resolve<ILogService>().GetLogger("自动发货");
                try
                {
                    var dict = new Dictionary<long, Tuple<long, string>>();
                    await isp.Resolve<IDataScope>().Retry("自动发货", async DataContext =>
                    {
                        foreach (var it in delivery.Items)
                        {
                            var re = await VIADDeliveryService.Delivery(new VIADDeliveryArgument
                            {
                                DeliveryId = delivery.Id,
                                DeliveryItemId = it.Id,
                                DeliveryItemName = it.Title,
                                PayloadId = it.PayloadId,
                                PayloadSpecId = it.PayloadSpecId,
                                SpecId = it.VIADSpecId,
                                Time = Time,
                                UserId = delivery.UserId
                            });
                            dict[it.Id] = re;
                        }
                        await VIADSupport.AutoDeliveryCompleted(delivery.Id, dict, Time);
                        return 0;
                    });
                    await VIADSupport.SendEvent(delivery.Id);
                    logger.Info($"自动发货成功: 发货:{delivery.Id} 卡密记录:{dict.Values.Select(v => v.Item1).Join(",")}");
                    return true;
                }
                catch (Exception e)
                {
                    logger.Error(e, "自动发货失败:" + e);
                    return false;
                }
            });
		}
		public static async Task Execute(IServiceProvider sp,int count)
		{
			var logger = sp.Resolve<ILogService>().GetLogger("自动发货");
			VIADDelivery[] deliveries=null;
            await sp.WithScope(async isp =>
            {
                var vdSupport = isp.Resolve<IVIADDeliverySupport>();
                deliveries = await vdSupport.Query(count);
            });
			logger.Info($"开始自动发货:待发货:{deliveries.Length}");
			if (deliveries == null || deliveries.Length == 0)
				return;
			var done = 0;
			foreach (var d in deliveries)
			{
				if (await Execute(sp, d))
					done++;
			}
			logger.Info($"完成自动发货:{done}");
		}
	}
}
