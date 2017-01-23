using System;

using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace SF.DI
{
	public static class MSDIExtensions
	{
		
		public static IDIServiceCollection GetDIServiceCollection(this IServiceCollection sc)
		{
			sc.AddTransient<IDIScopeFactory, SF.DI.Microsoft.ScopeFactory>();
			return new DIServcieCollection(sc);
		}
	}
}
