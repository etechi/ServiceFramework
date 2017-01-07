using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using SF.DI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SF.Reflection;
using SF.Services.Metadata;

namespace SF.AspNetCore.Mvc
{
	public class NetworkServiceConfig
	{
		public string RouterPrefix { get; set; } = "api";
		public IEnumerable<Type> ServiceTypes { get; set; } 
	}
	public static class MvcNetworkServiceExtensions
	{
		public static void UseMvcServiceInterface(
			this DI.IDIServiceCollection sc,
			NetworkServiceConfig cfg=null
			)
		{
			sc.AddSingleton<IActionDescriptorProvider>(sp =>
				new ServiceActionDescProvider(
					cfg?.RouterPrefix ?? "api",
					cfg?.ServiceTypes ??
					sc.ServiceTypes.Where(st => st.GetCustomAttribute<Annotations.NetworkServiceAttribute>() != null),
					sp.Resolve<IServiceBuildRuleProvider>()
					)
				);

			sc.Replace(new ServiceDescriptor(
				typeof(IControllerActivator),
				typeof(ServiceBasedControllerActivator),
				ServiceLifetime.Singleton
				));

		}
	}
}
