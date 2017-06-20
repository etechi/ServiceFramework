using System;
namespace SF.Core.ServiceManagement.Internals
{
	public interface IServiceInterfaceFactory
	{
		IServiceDeclaration ServiceDeclaration { get;}
		IServiceImplement ServiceImplement { get; }
		IServiceInterface ServiceInterface { get; }
		object Create(IServiceResolver ServiceResolver);
	}

}
