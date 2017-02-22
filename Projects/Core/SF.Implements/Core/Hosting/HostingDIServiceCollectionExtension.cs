using SF.Core.DI;
using SF.Core.ManagedServices.Runtime;
using SF.Core.ManagedServices.Storages;
using System.Linq;
using SF.Metadata;
using System;
using System.Reflection;
using SF.Core.Serialization;
using System.Collections.Generic;
using SF.Metadata.Models;
using System.ComponentModel.DataAnnotations;
using SF.Core.Hosting;
using SF.Core.ManagedServices.Admin;

namespace SF.Core.DI
{
	public static class HostingDIServiceCollectionService
	{
		public static IDIServiceCollection UseFilePathResolver(this IDIServiceCollection sc)
		{
			sc.AddScoped<IFilePathResolver, FilePathResolver>();
			sc.AddInitializer(
				"初始化文件路径解析服务",
				async sp =>
				{
					var sim = sp.Resolve<IServiceInstanceManager>();
					await sim.EnsureDefaultService<IFilePathResolver, FilePathResolver>(
						new FilePathDefination()
						);
				});
			return sc;
		}
	}

}
