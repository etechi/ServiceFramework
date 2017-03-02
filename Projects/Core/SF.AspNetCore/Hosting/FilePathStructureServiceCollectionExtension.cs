using Microsoft.AspNetCore.Hosting;
using SF.AspNetCore;
using SF.Core.DI;
using SF.Core.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SF.Core.DI
{
	public static class FilePathStructureServiceCollectionExtension 
	{
		public static IDIServiceCollection UseAspNetCoreFilePathStructure(this IDIServiceCollection sc)
		{
			sc.AddSingleton<IDefaultFilePathStructure, DefaultFilePathStructure>();
			return sc;
		}
	}
}
