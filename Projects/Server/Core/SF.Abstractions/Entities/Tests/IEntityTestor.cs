using SF.Core.ServiceManagement;
using SF.Core.ServiceManagement.Management;
using SF.Services.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Entities.Tests
{
	public interface IEntityTestDataProvider<TEditable>
	{
		Task<T> UseTestEntity<T>(int Count,Func<TEditable[], Task<T>> Action);
	}

	public interface IAutoTestEntity
	{
		Type EntityManagerType { get; }
	}
	public interface IAutoTestEntity<TManager> :IAutoTestEntity
	{ 
		Func<IServiceInstanceManager,IServiceInstanceInitializer<TManager>> ServiceConfig { get; }
	}
}
