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
		public static IServiceCollection AddConsoleDefaultFilePathStructure(this IServiceCollection sc)
		{
			sc.AddSingleton<IDefaultFilePathStructure, ConsoleDefaultFilePathStructure>();
			return sc;
		}
		public static IServiceCollection AddFilePathResolver(this IServiceCollection sc)
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
