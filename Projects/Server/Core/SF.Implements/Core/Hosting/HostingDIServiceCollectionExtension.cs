using SF.Core.Hosting;
using System.Linq.TypeExpressions;

namespace SF.Core.ServiceManagement
{
	public static class HostingDIServiceCollectionService
	{
		public static IServiceCollection AddConsoleDefaultFilePathStructure(this IServiceCollection sc)
		{
			sc.AddSingleton<IDefaultFilePathStructure, ConsoleDefaultFilePathStructure>();
			return sc;
		}
		public static IServiceCollection AddTestFilePathStructure(
			this IServiceCollection sc,
			string BinPath,
			string RootPath
			)
		{
			sc.AddSingleton<IDefaultFilePathStructure>(new TestFilePathStructure
			{
				BinaryPath = BinPath,
				RootPath = RootPath
			});
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
		public static IServiceCollection AddDynamicTypeBuilder(this IServiceCollection sc)
		{
			sc.AddSingleton<IDynamicTypeBuilder, DynamicTypeBuilder>();
			return sc;
		}
	}

}
