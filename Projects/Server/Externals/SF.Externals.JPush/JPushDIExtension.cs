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
			if (setting.Uri == null)
				setting.Uri = "https://api.jpush.cn/v3/push";
			sc.InitService("JPush", (sp, sim) =>
			{
				return sim.Service<INotificationSendProvider, NotificationSendProvider>(
						new
						{
							Setting = setting
						}
					).WithIdent("jpush");
			});
			return sc;
		}
        
    }
}
