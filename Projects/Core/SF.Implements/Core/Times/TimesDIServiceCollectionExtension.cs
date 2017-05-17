using SF.Core.DI;
using System.Linq;
using SF.Metadata;
using System;

namespace SF.Core.ServiceManagement
{
	public static class TimesDIServiceCollectionExtension
	{
		
		public static IServiceCollection UseSystemTimeService(
			this IServiceCollection sc
			)
		{
			sc.AddSingleton<Times.ITimeService, Times.TimeService>();
			return sc;
		}
	}

}
