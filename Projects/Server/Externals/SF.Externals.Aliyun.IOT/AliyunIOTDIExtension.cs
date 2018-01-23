using SF.Common.Notifications.Senders;
using SF.Externals.Aliyun;
using SF.Externals.Aliyun.Implements;

namespace SF.Sys.Services
{
	public static class AliyunIOTDIExtension
    {
        public static IServiceCollection AddAliyunIOTServices(
			this IServiceCollection sc,
			AliyunIOTSetting IOTSetting
			)
        {
			sc.AddManagedScoped<IAliyunIOTService, AliyunIOTService>();

			sc.InitServices("阿里云IOT服务",async (sp, sm, ParentId) =>
			{
				await sm.DefaultService<IAliyunIOTService, AliyunIOTService>(new { Setting= IOTSetting }).Ensure(sp, ParentId);

			});
			return sc;
		}
        
    }
}
