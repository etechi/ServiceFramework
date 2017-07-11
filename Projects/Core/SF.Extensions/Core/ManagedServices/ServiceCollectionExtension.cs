using System;
using System.Collections.Generic;

namespace SF.Core.ServiceManagement
{
	public static class ServiceCollectionExtension
	{
		public static bool IsManagedServiceImplement(this ServiceDescriptor sd)
		{
			if (sd.ServiceImplementType != ServiceImplementType.Type)
				return false;
			if (!sd.InterfaceType.IsInterface)
				return false;
			if (sd.InterfaceType.IsGenericType)
			{
				var gtd = sd.InterfaceType.GetGenericTypeDefinition();
				if (gtd == typeof(IEnumerable<>))
					return false;
			}
			return true;
		}
	}
}
