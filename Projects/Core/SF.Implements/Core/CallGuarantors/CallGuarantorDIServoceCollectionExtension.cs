using SF.Core.CallGuarantors;
using SF.Core.CallGuarantors.Runtime;
using SF.Core.CallGuarantors.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.Data;
namespace SF.Core.DI
{
	public static class CallGuarantorDIServiceCollectionExtension
	{

		public static IDIServiceCollection UseCallGuarantors(this IDIServiceCollection sc)
		{
			sc.AddSingleton(sp =>
				new CallableFactory(sp.Resolve<IEnumerable<ICallableDefination>>())
				);
			sc.AddScoped<ICallDispatcher, CallDispatcher>();
			sc.AddScoped<ICallGuarantor, CallGuarantor>();

			return sc;
		}

		public static IDIServiceCollection UseCallGuarantorStorage(this IDIServiceCollection sc,string TablePrefix=null)
		{
			sc.UseDataModules<SF.Core.CallGuarantors.Storage.DataModels.CallExpired, SF.Core.CallGuarantors.Storage.DataModels.CallInstance>(TablePrefix);
			sc.AddScoped<ICallGuarantorStorage, CallGuarantorStorage>();
			return sc;
		}
	}
}
