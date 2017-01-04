using System;
namespace SF.ServiceManagement
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

}
