using System.Threading.Tasks;
using SF.Common.Notifications.Senders;
using SF.Externals.JPush;
using SF.Sys.NetworkService;

namespace SF.Sys.Services
{
	
	public static class JPushDIExtension
    {
        public static IServiceCollection AddJPushServices(this IServiceCollection sc,JPushSetting setting)
        {

			sc.AddManagedScoped<INotificationSendProvider, NotificationSendProvider>();
			sc.InitServices("JPush", async (sp, sim, parent) =>
			{
				setting=await sp.LoadServiceSetupSetting(
					setting,
					new JPushSetting
					{
						Uri = "https://api.jpush.cn/v3/push"
					});

				await sim.Service<INotificationSendProvider, NotificationSendProvider>(
						new
						{
							Setting = setting
						}
					).WithIdent("jpush").Ensure(sp,parent);
			});
			return sc;
		}
        
    }
}
