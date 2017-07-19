using System;
namespace SF.Core.ServiceManagement.Internals
{
	public interface IServiceFactory : IServiceInstanceDescriptor
	{
		object Create(IServiceResolver ServiceResolver);
	}

}
