using System;

using System.Collections.Generic;

namespace SF.ServiceManagement.Internal
{
	interface IServiceMetadata: IServiceDetector
	{
		IEnumerable<Type> NormalServices { get; }
		IEnumerable<KeyValuePair<Type, IReadOnlyList<Type>>> ManagedServices { get; }
	}

}
