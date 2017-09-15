using SF.Core.ServiceManagement.Internals;
using System;
using System.Collections.Generic;
namespace SF.Core.ServiceManagement
{
	
	static class Types
	{
		public static Type ServiceProviderType { get; } = typeof(IServiceProvider);
		public static Type ServiceResolverType { get; } = typeof(IServiceResolver);
		public static Type ServiceInstanceDescriptorType { get; } = typeof(IServiceInstanceDescriptor);

	}

}
