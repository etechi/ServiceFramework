using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SF.Core.ServiceManagement;
using SF.AspNetCore.NetworkServices;
using SF.Metadata;
using SF.Core.NetworkService;

namespace SF.Core.DI
{
	public class NetworkServiceConfig
	{
		public string RouterPrefix { get; set; } = "api";
	}
	public static class MvcNetworkServiceExtensions
	{
		public static void UseAspNetCoreServiceInterface(
			this IServiceCollection sc,
			NetworkServiceConfig cfg=null
			)
		{
			sc.AddScoped<IInvokeContext, InvokeContext>();
			sc.AddScoped<IUploadedFileCollection, UploadedFileCollection>();
			sc.AddSingleton<IActionDescriptorProvider>(sp =>
				new ServiceActionDescProvider(
					cfg?.RouterPrefix ?? "api",
					sp.Resolve<IServiceTypeCollection>().Types,
					sp.Resolve<IServiceBuildRuleProvider>()
					)
				);

			sc.Replace(new ServiceDescriptor(
				typeof(IControllerActivator),
				typeof(SF.AspNetCore.NetworkServices.ServiceBasedControllerActivator),
				ServiceImplementLifetime.Singleton
				));

			sc.AddSingleton<IResultFactory, ResultFactory>();
		}
	}
}
