using System.Linq;
using SF.Metadata;
using System;
using SF.Core.ServiceManagement;
namespace SF.Core.ServiceManagement
{
	public static class SysMemCachingDIServiceCollectionExtension
	{
		public static IServiceCollection UseSystemMemoryCache(
			this IServiceCollection sc
			)
		{
			sc.AddSingleton(typeof(Caching.ILocalCache<>),typeof(Caching.SystemMemoryCache<>));
			return sc;
		}
	}

}
