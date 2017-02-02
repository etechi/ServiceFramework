using System;

using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace SF.Core.DI.MicrosoftExtensions
{
	public static class MSDIExtensions
	{
		public static IDIServiceCollection GetDIServiceCollection(this IServiceCollection sc)
		{
			sc.AddTransient<IDIScopeFactory, ScopeFactory>();
			return new DIServiceCollection(sc);
		}
	
	}
}
