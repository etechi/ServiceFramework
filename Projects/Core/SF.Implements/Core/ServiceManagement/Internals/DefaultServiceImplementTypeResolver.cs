using System;
namespace SF.Core.ServiceManagement.Internals
{

	public class DefaultServiceImplementTypeResolver : IServiceImplementTypeResolver
	{
		public string GetTypeIdent(Type type)
		{
			return type.FullName;
		}

		public Type Resolve(string Name)
		{
			return Type.GetType(Name);
		}
	}
	public class DefaultServiceDeclarationTypeResolver : IServiceDeclarationTypeResolver
	{
		public string GetTypeIdent(Type type)
		{
			return type.FullName;
		}

		public Type Resolve(string Name)
		{
			return Type.GetType(Name);
		}
	}
}
