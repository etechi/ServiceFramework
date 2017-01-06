using System;
namespace SF.Services.Management
{
	public interface IManagedServiceScope
	{
		object Resolve(IServiceProvider sp, Type Type, string Id);
	}
}
