using SF.Core.DI;
using SF.Core.ManagedServices.Runtime;
using SF.Core.ManagedServices.Storages;
using System.Linq;
using SF.Metadata;
using System;
using SF.Core.ManagedServices.Admin;

namespace SF.Core.DI
{
	public static class CachingDIServiceCollectionExtension
	{
		public static IDIServiceCollection UseLocalFileCache(
			this IDIServiceCollection sc
			)
		{
			sc.AddScoped<Caching.IFileCache, Caching.LocalFileCache>();
			sc.AddInitializer(
				"��ʼ���ļ��������",
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
