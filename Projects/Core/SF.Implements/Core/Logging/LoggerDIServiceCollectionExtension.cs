using SF.Core.DI;
using System.Linq;
using SF.Metadata;
using System;
using System.Reflection;
using SF.Core.Serialization;
using System.Collections.Generic;
using SF.Metadata.Models;
using System.ComponentModel.DataAnnotations;
using SF.Core.Logging;

namespace SF.Core.ServiceManagement
{
	public static class LoggerDIServiceCollectionService
	{
		public static IServiceCollection AddLogService(this IServiceCollection sc,ILogService LogService=null)
		{
			sc.AddSingleton<ILogService>(LogService??new LogService());
			sc.Add(new ServiceDescriptor(typeof(ILogger<>), typeof(Logger<>), ServiceImplementLifetime.Scoped));
			return sc;
		}
	}

}
