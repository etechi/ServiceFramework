using SF.Core.DI;
using SF.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http.Dispatcher;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Linq.Expressions;
using SF.Services.NetworkService;
using SF.AspNet.NetworkService;

namespace SF.Core.DI
{
	public class NetworkServiceConfig
	{
		public string RouterPrefix { get; set; } = "api";
		public IEnumerable<Type> ServiceTypes { get; set; }
	}
	public static class NetworkServiceDIServiceCollectionExtensions
	{
		public static void UseWebApiNetworkService(
			this IDIServiceCollection sc,
			HttpConfiguration HttpConfiguration,
			NetworkServiceConfig ServiceConfig=null
			)
		{
			//sc.AddSingleton<IHttpControllerTypeResolver>(sp =>
			//	new ServiceApiControllerTypeResolver(
			//		ServiceConfig?.RouterPrefix ?? "api",
			//		sp.Resolve<IServiceTypeCollection>().Types,
			//		sp.Resolve<IServiceBuildRuleProvider>()
			//		)
			//	);

			sc.AddSingleton<IHttpControllerActivator>(sp =>
				new ServiceApiControllerActivator(sp.Resolve<IServiceTypeCollection>().Types)
				);
			sc.AddSingleton<IHttpActionSelector>(sp =>
				 new ServiceHttpActionSelector(
					 sp.Resolve<IServiceBuildRuleProvider>(),
					sp.Resolve<IServiceTypeCollection>().Types
					 )
				 );
			sc.AddSingleton<IHttpControllerSelector>(sp =>
				new ServiceApiControllerSelector(
					HttpConfiguration,
					sp.Resolve<IServiceTypeCollection>().Types,
					sp.Resolve<IServiceBuildRuleProvider>()
				));
		}
	}
}
