using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Core.ServiceManagement
{
	public static class ServiceMetadataExtension
	{
		public static IServiceDeclaration FindServiceByType(this IServiceMetadata metadata,Type type)
		{
			var s = metadata.Services.Get(type);
			if (s != null || !type.IsGenericType) return s;
			return metadata.Services.Get(type.GetGenericTypeDefinition());
		}
		public static IServiceImplement FindImplementByType(this IServiceMetadata metadata, Type ServiceType,Type ImplementType)
		{
			var svc = metadata.FindServiceByType(ServiceType);
			if (svc == null)
				return null;
			return svc.Implements.SingleOrDefault(i => i.ImplementType == ImplementType);
		}
	}
}
