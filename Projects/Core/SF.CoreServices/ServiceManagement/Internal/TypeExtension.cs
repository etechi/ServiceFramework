using System;
using System.Reflection;
namespace SF.ServiceManagement.Internal
{
	static class TypeExtension
	{
		public static bool IsInterfaceType(this Type type)
		{
			return type.GetTypeInfo().IsInterface;
		}
	}
}
