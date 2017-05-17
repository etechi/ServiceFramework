using SF.Core.DI;
using System.Linq;
using SF.Metadata;
using System;
using SF.Core.ServiceManagement;
using SF.Core.ServiceManagement.Management;

namespace SF.Core.ServiceManagement
{
	public static class CachingDIServiceCollectionExtension
	{
		public static IServiceCollection UseLocalFileCache(
			this IServiceCollection sc
			)
		{
			sc.AddScoped<Caching.IFileCache, Caching.LocalFileCache>();
			sc.AddInitializer(
				"初始化文件缓存服务",
				async sp =>
				{
					var sim = sp.Resolve<IServiceInstanceManager>();
					await sim.EnsureDefaultService<Caching.IFileCache, Caching.LocalFileCache>(
						new
						{
							Setting = new
							{
								RootPath = "temp://cache",
								PathResolver = await sim.ResolveDefaultService<Hosting.IFilePathResolver>()
							}
						});
				});
			return sc;
		}
	}

}
