using Microsoft.AspNetCore.Hosting;
using SF.AspNetCore;
using SF.Core.ServiceManagement;
using SF.Core.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SF.Core.ServiceManagement
{
	public static class FilePathStructureServiceCollectionExtension 
	{
		public static IServiceCollection AddAspNetCoreFilePathStructure(this IServiceCollection sc)
		{
			sc.AddSingleton<IDefaultFilePathStructure, DefaultFilePathStructure>();
			return sc;
		}
	}
}
