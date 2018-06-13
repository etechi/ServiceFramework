using System.Threading.Tasks;
using SF.Common.Notifications.Senders;
using SF.Externals.Aliyun.SMS;
using SF.Sys.NetworkService;

namespace SF.Sys.Services
{
	
	public static class AliyunSMSDIExtension
    {
        public static IServiceCollection AddAliyunSMSServices(this IServiceCollection sc,AliyunSMSSetting setting=null)
        {
			sc.AddManagedScoped<INotificationSendProvider, SMSSendProvider>();
			sc.InitServices("AliyunSMS", async (sp, sim,parent) =>
			{
				setting = await sp.LoadServiceSetupSetting(setting);
				await sim.Service<INotificationSendProvider, SMSSendProvider>(
						new
						{
							Setting = setting
						}
					).WithIdent("aliyun-sms").Ensure(sp,parent);
			});
			return sc;
		}
        
    }
}
