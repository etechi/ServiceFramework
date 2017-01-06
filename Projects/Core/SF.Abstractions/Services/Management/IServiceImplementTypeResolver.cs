using System;

namespace SF.Services.Management
{
	public interface IServiceImplementTypeResolver
	{
		string GetTypeIdent(Type type);
		Type Resolve(string Name);
	}

}
