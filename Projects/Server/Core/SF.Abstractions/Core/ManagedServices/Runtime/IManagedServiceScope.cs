using System;
namespace SF.Core.ManagedServices.Runtime
{
	public interface IManagedServiceScope
	{
		object Resolve( Type Type, string Id);
	}
}
