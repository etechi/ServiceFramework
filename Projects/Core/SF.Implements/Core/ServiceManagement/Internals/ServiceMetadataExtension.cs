using SF.Core.DI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SF.Core.ServiceManagement.Internals
{
	public static class ServiceMetadataExtension
	{
		//public static IServiceDeclaration ResolveServiceImplements(this IServiceMetadata ServiceMetadata,Type ServiceType)
		//{
		//	var decl = ServiceMetadata.Services.Get(ServiceType);
		//	if (decl != null)
		//		return decl;
		//	if (!ServiceType.IsGenericType)
		//		return null;

		//	var typeDef = ServiceType.GetGenericTypeDefinition();
		//	decl = ServiceMetadata.Services.Get(typeDef);
		//	if (decl != null)
		//		return decl;

		//	return null;
		//}
	}

}
