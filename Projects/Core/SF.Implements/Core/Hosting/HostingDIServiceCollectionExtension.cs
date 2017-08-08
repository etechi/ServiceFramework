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
		public static IServiceCollection UseConsoleDefaultFilePathStructure(this IServiceCollection sc)
		{
			sc.AddSingleton<IDefaultFilePathStructure, ConsoleDefaultFilePathStructure>();
			return sc;
		}
		public static IServiceCollection UseFilePathResolver(this IServiceCollection sc)
		{
			sc.AddManagedScoped<IFilePathResolver, FilePathResolver>();
			sc.InitDefaultService<IFilePathResolver, FilePathResolver>(
				"��ʼ���ļ�·����������",
				new
				{
					Setting = new FilePathDefination()
				}
				);
			return sc;
		}
	}

}
