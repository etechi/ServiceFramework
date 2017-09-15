using SF.Core.Hosting;

namespace SF.Core.ServiceManagement
{
	public static class HostingDIServiceCollectionService
	{
		public static IServiceCollection AddConsoleDefaultFilePathStructure(this IServiceCollection sc)
		{
			sc.AddSingleton<IDefaultFilePathStructure, ConsoleDefaultFilePathStructure>();
			return sc;
		}
		public static IServiceCollection AddFilePathResolver(this IServiceCollection sc)
		{
			sc.AddManagedScoped<IFilePathResolver, FilePathResolver>();
			sc.InitDefaultService<IFilePathResolver, FilePathResolver>(
				"初始化文件路径解析服务",
				new
				{
					Setting = new FilePathDefination()
				}
				);
			return sc;
		}
	}

}
