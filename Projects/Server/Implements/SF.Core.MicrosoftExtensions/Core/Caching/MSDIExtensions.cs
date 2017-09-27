using System;

namespace SF.Core.ServiceManagement
{
	public static class MSCachingExtensions
	{
		public static IServiceCollection AddMicrosoftMemoryCacheAsLocalCache(this IServiceCollection sc)
		{
			return sc.AddSingleton(
				typeof(SF.Core.Caching.ILocalCache<>),
				typeof(SF.Core.Caching.MicrosoftExtensions.LocalCache<>)
				);
		}
	
	}
}
