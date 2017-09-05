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
				"��ʼ���ļ�·����������",
				new
				{
					Setting = new FilePathDefination()
				}
				);
			return sc;
		}
	}

}
