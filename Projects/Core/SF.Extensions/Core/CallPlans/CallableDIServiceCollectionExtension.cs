using SF.Core.CallPlans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.Core.ServiceManagement;
namespace SF.Core.ServiceManagement
{
	
	public static class CallPlansDIServiceCollectionExtension
	{
		class CallableDefination : ICallableDefination
		{
			public Func<IServiceProvider,long?, ICallable> CallableCreator{get;set;}
			public string Type { get; set; }
		}

		public static void AddCallable<T>(this IServiceCollection sc)
			where T: class,ICallable
		{
			sc.AddScoped<T, T>();
			sc.AddSingleton<ICallableDefination>(sp => new CallableDefination
			{
				Type = typeof(T).FullName,
				CallableCreator = (isp,id) => (ICallable)isp.Resolve<T>()
			});
		}
		public static void AddServiceCallable<I>(this IServiceCollection sc)
			where I : class
		{
			sc.AddSingleton<ICallableDefination>(sp => {
				var svcResolver = sp.Resolve<IServiceDeclarationTypeResolver>();
				return new CallableDefination
				{
					Type = svcResolver.GetTypeIdent(typeof(I)),
					CallableCreator = (isp, id) =>
						(ICallable)isp.Resolve<I>(id.Value)
				};
			});
		}
	}
}
