using System;
namespace SF.Services.ManagedServices.Runtime
{
	public interface IManagedServiceScope
	{
		object Resolve( Type Type, string Id);
	}
}
