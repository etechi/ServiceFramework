using System;
namespace SF.Core.ManagedServices.Runtime
{
	public interface IManagedServiceFactory
	{
		bool IsScopedLifeTime { get; }
		object Create(IServiceProvider ServiceProvider, IManagedServiceScope ManagedServiceScope);
	}
	public interface IManagedServiceFactoryManager
	{
		IManagedServiceFactory GetServiceFactory(IServiceProvider ServiceProvider, Type Type, string Id);
	}

}
