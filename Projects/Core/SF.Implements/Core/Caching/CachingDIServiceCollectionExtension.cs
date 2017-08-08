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
			sc.AddManagedScoped<Caching.IFileCache, Caching.LocalFileCache>();
			sc.InitDefaultService<Caching.IFileCache, Caching.LocalFileCache>(
				"��ʼ���ļ��������",
				new
				{
					Setting = new
					{
						RootPath = "temp://cache",
					}
				}
				);
			return sc;
		}
	}

}
