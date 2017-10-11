using Microsoft.AspNetCore.Hosting;
using SF.AspNetCore;
using SF.Core.ServiceManagement;
using SF.Core.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using SF.Clients;

namespace SF.Core.ServiceManagement
{
	public static class AspNetCoreHostingServiceCollectionExtension 
	{
		public static IServiceCollection AddAspNetCoreFilePathStructure(this IServiceCollection sc)
		{
			sc.AddSingleton<IDefaultFilePathStructure, DefaultFilePathStructure>();
			return sc;
		}
		public static IServiceCollection AddAspNetCoreClientService(this IServiceCollection sc)
		{
			sc.AddScoped<IClientService, AspNetCoreClientService>();
			return sc;
		}
		public static IServiceCollection AddAspNetCoreHostingService(this IServiceCollection sc)
		{
			sc.AddAspNetCoreClientService();
			sc.AddAspNetCoreFilePathStructure();
			return sc;
		}
	}
}
