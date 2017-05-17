using SF.Core.CallGuarantors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.Core.ServiceManagement;
namespace SF.Core.ServiceManagement
{
	
	public static class CallGuarantorDIServiceCollectionExtension
	{
		class CallableDefination : ICallableDefination
		{
			public Func<IServiceProvider, ICallable> CallableCreator{get;set;}
			public Type Type { get; set; }
		}

		public static void AddCallable<T>(this IServiceCollection sc)
			where T: class,ICallable
		{
			sc.AddScoped<T, T>();
			sc.AddSingleton<ICallableDefination>(sp => new CallableDefination
			{
				Type = typeof(T),
				CallableCreator = (isp) => (ICallable)isp.Resolve<T>()
			});
		}
	}
}
