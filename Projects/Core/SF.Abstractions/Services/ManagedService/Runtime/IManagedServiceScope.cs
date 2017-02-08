using System;
namespace SF.Services.ManagedServices.Runtime
{
	public interface IManagedServiceScope
	{
		object Resolve(IServiceProvider sp, Type Type, string Id);
	}
}
