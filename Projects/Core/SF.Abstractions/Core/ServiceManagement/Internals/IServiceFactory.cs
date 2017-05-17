using System;
namespace SF.Core.ServiceManagement.Internals
{
	public interface IServiceFactory
	{
		IServiceDeclaration ServiceDeclaration { get;}
		IServiceImplement ServiceImplement { get; }
		object Create(IServiceResolver ServiceResolver);
	}

}
