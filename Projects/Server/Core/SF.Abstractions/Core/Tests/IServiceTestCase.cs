using SF.Core.ServiceManagement.Management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Core.Tests
{
	public interface IServiceTestCase
	{
		string Name { get; }
	}
	public interface ITestContext
	{
		IServiceProvider ServiceProvider { get; }
	}
	public interface IServiceTestCase<TService>
	{
		Task Execute(ITestContext Context, TService Service);
	}
}
