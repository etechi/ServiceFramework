using System;
namespace SF.Core.ServiceManagement.Internals
{
	public interface IServiceFactory
	{
		long ServiceInstanceId { get; }
		IServiceDeclaration ServiceDeclaration { get;}
		IServiceImplement ServiceImplement { get; }
		object Create(IServiceProvider ServiceProvider);
	}

}
