using SF.Core.ServiceFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SF.Core.DI;
namespace SF.Core.ServiceFeatures
{
	public static class InitializableServiceProviderExtension
	{
		public static async Task InitServices(this IServiceProvider sp)
		{
			var bss=sp.Resolve<IEnumerable<IServiceInitializable>>();
			foreach (var bs in bss)
				await bs.Init();
		}

	}
}
