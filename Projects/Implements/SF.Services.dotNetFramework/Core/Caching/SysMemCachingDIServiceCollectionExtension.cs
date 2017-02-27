using SF.Core.DI;
using System.Linq;
using SF.Metadata;
using System;
using SF.Core.ManagedServices.Admin;

namespace SF.Core.DI
{
	public static class SysMemCachingDIServiceCollectionExtension
	{
		public static IDIServiceCollection UseSystemMemoryCache(
			this IDIServiceCollection sc
			)
		{
			sc.AddSingleton<Caching.ILocalCache>(new Caching.SystemMemoryCache("DefaultCache"));
			return sc;
		}
	}

}
