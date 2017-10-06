using System.Threading.Tasks;
using System;
using SF.Core.ServiceManagement;
using System.Linq;
using SF.Data;
using System.Reflection;
using System.Linq.Expressions;
using System.Data.Common;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using SF.Core.ServiceManagement.Management;

namespace SF.Entities.Tests
{
	public static class TestDIExtension
	{
		class AutoTestEntity<T> : IAutoTestEntity<T>
		{
			public Func<IServiceInstanceManager, IServiceInstanceInitializer<T>> ServiceConfig { get; set; }

			public Type EntityManagerType => typeof(T);
		}

		public static IServiceCollection AddAutoEntityTest<TManager>(
			this IServiceCollection sc,
			Func<IServiceInstanceManager,IServiceInstanceInitializer<TManager>> ServiceConfig
			)
		{
			sc.AddSingleton<IAutoTestEntity>(new AutoTestEntity<TManager>
			{
				ServiceConfig=ServiceConfig
			});
			return sc;
		}
	}
}
