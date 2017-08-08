using System;

namespace SF.Core.ServiceManagement
{
	public interface IServiceImplementTypeResolver
	{
		string GetTypeIdent(Type type);
		Type Resolve(string Name);
	}


	public interface IServiceDeclarationTypeResolver
	{
		string GetTypeIdent(Type type);
		Type Resolve(string Name);
	}
}
