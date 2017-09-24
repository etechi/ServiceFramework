using System;

using System.Collections.Generic;
using SF.Core.ServiceManagement;

namespace SF.Core.DI
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
