using SF.Core.DI;
using SF.Core.ManagedServices.Runtime;
using SF.Core.ManagedServices.Storages;
using System.Linq;
using SF.Metadata;
using System;

namespace SF.Core.DI
{
	public static class TimesDIServiceCollectionExtension
	{
		
		public static IDIServiceCollection UseSystemTimeService(
			this IDIServiceCollection sc
			)
		{
			sc.AddSingleton<Times.ITimeService, Times.TimeService>();
			return sc;
		}
	}

}
