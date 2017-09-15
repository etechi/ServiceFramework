using System;

using System.Collections.Generic;

namespace SF.Services.Management.Internal
{
	interface IServiceMetadata: IServiceDetector
	{
		IEnumerable<Type> NormalServices { get; }
		IEnumerable<KeyValuePair<Type, IReadOnlyList<Type>>> ManagedServices { get; }
	}

}
