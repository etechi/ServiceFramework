using System;
namespace SF.ServiceManagement
{
	public interface IManagedServiceScope
	{
		object Resolve(IServiceProvider sp, Type Type, string Id);
	}
}
