using System;

using System.Collections.Generic;

namespace SF.Services.ManagedServices.Runtime
{
	public interface IManagedServiceDescriptor
	{
		Type Type { get; }
		bool IsScopedLifeTime { get; }
	}
	public interface IServiceMetadata : IServiceDetector
	{
		IEnumerable<Type> NormalServices { get; }
		IReadOnlyDictionary<Type, IReadOnlyList<IManagedServiceDescriptor>> ManagedServices { get; }
	}

}
