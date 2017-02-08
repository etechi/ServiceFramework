using System;

using System.Collections.Generic;

namespace SF.Services.ManagedServices.Runtime
{
	interface IImple: IServiceDetector
	{
		IEnumerable<Type> NormalServices { get; }
		IEnumerable<KeyValuePair<Type, IReadOnlyList<Type>>> ManagedServices { get; }
	}

}
