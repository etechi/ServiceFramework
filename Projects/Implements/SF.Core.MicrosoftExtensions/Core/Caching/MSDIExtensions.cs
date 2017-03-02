using System;

using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace SF.Core.DI
{
	public static class MSCachingExtensions
	{
		public static IDIServiceCollection UseMicrosoftMemoryCacheAsLocalCache(this IDIServiceCollection sc)
		{
			return sc.AddSingleton<SF.Core.Caching.ILocalCache, SF.Core.Caching.MicrosoftExtensions.LocalCache>();
		}
	
	}
}
