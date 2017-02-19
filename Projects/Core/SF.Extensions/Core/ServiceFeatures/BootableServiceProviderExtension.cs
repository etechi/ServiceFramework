using SF.Core.ServiceFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SF.Core.DI;

namespace SF.Core.ServiceFeatures
{
	public static class StartBootstrapServiceProviderExtension
	{
		public static async Task<IDisposable> BootServices(this IServiceProvider sp)
		{
			var ds = new List<IDisposable>();
			var bss=sp.Resolve<IEnumerable<IServiceBootable>>();
			foreach (var bs in bss)
				ds.Add(await bs.Boot());
			return Disposable.Combine(ds.ToArray());
		}

	}
}
