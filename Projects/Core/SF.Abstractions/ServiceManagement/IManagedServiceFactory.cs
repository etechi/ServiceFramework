using System;
namespace SF.ServiceManagement
{
	public interface IManagedServiceFactory
	{
		object Create(IServiceProvider ServiceProvider, IManagedServiceScope ManagedServiceScope, Type Type, string Id);
	}

}
