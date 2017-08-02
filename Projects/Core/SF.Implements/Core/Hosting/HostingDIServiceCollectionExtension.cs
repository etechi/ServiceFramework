using SF.Core.DI;
using System.Linq;
using SF.Metadata;
using System;
using System.Reflection;
using SF.Core.Serialization;
using System.Collections.Generic;
using SF.Metadata.Models;
using System.ComponentModel.DataAnnotations;
using SF.Core.Hosting;
using SF.Core.ServiceManagement;
using SF.Core.ServiceManagement.Management;

namespace SF.Core.ServiceManagement
{
	public static class HostingDIServiceCollectionService
	{
		public static IServiceCollection UseFilePathResolver(this IServiceCollection sc)
		{
			sc.AddScoped<IFilePathResolver, FilePathResolver>();
			sc.InitDefaultService<IFilePathResolver, FilePathResolver>(
				"初始化文件路径解析服务",
				new FilePathDefination()
				);
			return sc;
		}
	}

}
