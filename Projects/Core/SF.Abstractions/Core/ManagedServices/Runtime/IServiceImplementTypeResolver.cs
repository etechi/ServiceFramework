using System;

namespace SF.Core.ManagedServices.Runtime
{
	public interface IServiceImplementTypeResolver
	{
		string GetTypeIdent(Type type);
		Type Resolve(string Name);
	}

}
