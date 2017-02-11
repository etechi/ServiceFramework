using SF.Core.DI;
using SF.Core.ManagedServices.Runtime;
using SF.Core.ManagedServices.Storages;
using System.Linq;
using SF.Metadata;
using System;

namespace SF.Core.DI
{
	public static class CachingDIServiceCollectionExtension
	{
		
		public static IDIServiceCollection UseSystemMemoryCache(
			this IDIServiceCollection sc
			)
		{
			sc.AddSingleton<Caching.ILocalCache, Caching.SystemMemoryCache>();
			return sc;
		}
		public static IDIServiceCollection UseLocalFileCache(
			this IDIServiceCollection sc
			)
		{
			sc.AddSingleton<Caching.IFileCache, Caching.LocalFileCache>();
			return sc;
		}
	}

}
