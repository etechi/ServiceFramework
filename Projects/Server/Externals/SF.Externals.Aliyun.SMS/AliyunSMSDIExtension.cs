using System.Threading.Tasks;
using SF.Common.Notifications.Senders;
using SF.Externals.Aliyun.SMS;
using SF.Sys.NetworkService;

namespace SF.Sys.Services
{
	
	public static class AliyunSMSDIExtension
    {
        public static IServiceCollection AddAliyunSMSServices(this IServiceCollection sc,AliyunSMSSetting setting)
        {
			sc.AddManagedScoped<INotificationSendProvider, SMSSendProvider>();
			sc.InitService("AliyunSMS", (sp, sim) =>
			{
				return sim.Service<INotificationSendProvider, SMSSendProvider>(
						new
						{
							Setting = setting
						}
					).WithIdent("aliyun-sms");
			});
			return sc;
		}
        
    }
}
