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
	public static class AspNetCoreServiceCollectionExtension 
	{
		public static IServiceCollection AddAspNetCore(this IServiceCollection sc)
		{
			return sc.AddAspNetCoreHostingService()
				.AddFrontSideMvcContentRenderProvider();
		}
	}
}
