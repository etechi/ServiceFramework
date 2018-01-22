using SF.Common.Notifications.Senders;
using SF.Externals.Aliyun;

namespace SF.Sys.Services
{
	public static class AliyunDIExtension
    {
        public static IServiceCollection AddAliyunServices(this IServiceCollection sc,AliyunSetting setting)
        {

			return sc;
		}
        
    }
}
