using SF.Common.Notifications.Senders;
using SF.Externals.Aliyun;
using SF.Externals.Aliyun.Implements;

namespace SF.Sys.Services
{
	public static class AliyunDIExtension
    {
        public static IServiceCollection AddAliyunServices(
			this IServiceCollection sc,
			AliyunSetting Setting=null
			)
        {
			sc.AddManagedScoped<IAliyunInvoker, AliyunInvoker>();
			
			sc.InitServices("阿里云服务",async (sp, sm, ParentId) =>
			{
				Setting = await sp.LoadServiceSetupSetting(Setting);
				await sm.DefaultService<IAliyunInvoker, AliyunInvoker>(new { Setting }).Ensure(sp,ParentId);
			
			});
			return sc;
		}
        
    }
}
