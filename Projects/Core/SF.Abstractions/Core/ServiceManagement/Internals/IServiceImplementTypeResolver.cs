using System;

namespace SF.Core.ServiceManagement.Internals
{
	[UnmanagedService]
	public interface IServiceImplementTypeResolver
	{
		string GetTypeIdent(Type type);
		Type Resolve(string Name);
	}

}
