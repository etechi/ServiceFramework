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
using System.Threading.Tasks;

namespace SF.UT
{
	public class TestBase : IDisposable
	{
		public IAppInstance AppInstance { get; }
		public Task<T> Scope<A0,T>(Func<A0, Task<T>> Action)
			=> AppInstance.ServiceProvider.WithScope(Action);
		public Task<T> Scope<A0, A1, T>(Func<A0, A1, Task<T>> Action)
			=> AppInstance.ServiceProvider.WithScope(Action);
		public Task<T> Scope<A0, A1, A2, T>(Func<A0, A1, A2, Task<T>> Action)
			=> AppInstance.ServiceProvider.WithScope(Action);
		public Task<T> Scope<A0, A1, A2, A3, T>(Func<A0, A1, A2, A3, Task<T>> Action)
			=> AppInstance.ServiceProvider.WithScope(Action);

		public TestBase()
		{
			AppInstance = TestApp.Builder(EnvironmentType.Development).Build();
		}

		public void Dispose()
		{
			AppInstance.Dispose();
		}
	}
	

}
