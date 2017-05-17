using System;

namespace SF.Core.ServiceManagement.Internals
{ 
	public interface IServiceImplementTypeResolver
	{
		string GetTypeIdent(Type type);
		Type Resolve(string Name);
	}

}
