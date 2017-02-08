using System;

namespace SF.Services.ManagedServices.Runtime
{
	public interface IServiceImplementTypeResolver
	{
		string GetTypeIdent(Type type);
		Type Resolve(string Name);
	}

}
