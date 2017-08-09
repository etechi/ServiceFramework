using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SF.Data.Storage;
using Xunit;
using SF.Applications;
using SF.Core.Hosting;
using SF.Core.ServiceManagement;
namespace SF.UT
{
	public class TestBase : IDisposable
	{
		public IAppInstance AppInstance { get; }
		public IServiceProvider ServiceProvider { get; }
		public IServiceScope ServiceScope { get; }
		public TestBase()
		{
			AppInstance = App.Builder(EnvironmentType.Development).Build();
			ServiceScope = AppInstance.ServiceProvider.Resolve<IServiceScopeFactory>().CreateServiceScope();
			ServiceProvider = ServiceScope.ServiceProvider;
		}

		public void Dispose()
		{
			ServiceScope.Dispose();
			AppInstance.Dispose();
		}
	}
	

}
