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
using SF.Core.Logging;

namespace SF.Core.DI
{
	public static class LoggerDIServiceCollectionService
	{
		public static IDIServiceCollection AddLogService(this IDIServiceCollection sc,ILogService LogService=null)
		{
			sc.AddSingleton<ILogService>(LogService??new LogService());
			sc.Add(new ServiceDescriptor(typeof(ILogger<>), typeof(Logger<>), ServiceLifetime.Scoped));
			return sc;
		}
	}

}
