using System;
using System.Reflection;

namespace SF.Core.ServiceManagement.Internals
{

	public class DefaultServiceImplementTypeResolver : IServiceImplementTypeResolver
	{
		IServiceMetadata Metadata { get; }
		public DefaultServiceImplementTypeResolver(IServiceMetadata meta)
		{
			Metadata = meta;
		}
		public string GetTypeIdent(Type type)
		{
			return type.GetFullName();
		}

		public Type Resolve(string Name)
		{
			return Metadata.ImplementsByTypeName.TryGetValue(Name, out var impls) ? impls[0].ImplementType : null;
		}
	}
	public class DefaultServiceDeclarationTypeResolver : IServiceDeclarationTypeResolver
	{
		IServiceMetadata Metadata { get; }
		public DefaultServiceDeclarationTypeResolver(IServiceMetadata meta)
		{
			Metadata = meta;
		}
		public string GetTypeIdent(Type type)
		{
			return type.GetFullName();
		}

		public Type Resolve(string Name)
		{
			return Metadata.ServicesByTypeName.TryGetValue(Name, out var impl) ? impl.ServiceType: null;
		}
	}
}
