using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SF.Core.DI;
using SF.AspNetCore.Mvc;
using SF.Metadata;
using SF.Services.NetworkService;

namespace SF.Core.DI
{
	public class NetworkServiceConfig
	{
		public string RouterPrefix { get; set; } = "api";
	}
	public static class MvcNetworkServiceExtensions
	{
		public static void UseMvcServiceInterface(
			this IDIServiceCollection sc,
			NetworkServiceConfig cfg=null
			)
		{
			sc.AddSingleton<IActionDescriptorProvider>(sp =>
				new ServiceActionDescProvider(
					cfg?.RouterPrefix ?? "api",
					sp.Resolve<IServiceTypeCollection>().Types,
					sp.Resolve<IServiceBuildRuleProvider>()
					)
				);

			sc.Replace(new ServiceDescriptor(
				typeof(IControllerActivator),
				typeof(SF.AspNetCore.Mvc.ServiceBasedControllerActivator),
				ServiceLifetime.Singleton
				));

		}
	}
}
