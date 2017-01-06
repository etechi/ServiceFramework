using System;
namespace SF.Services.Management
{
	public interface IManagedServiceFactory
	{
		object Create(IServiceProvider ServiceProvider, IManagedServiceScope ManagedServiceScope, Type Type, string Id);
	}

}
