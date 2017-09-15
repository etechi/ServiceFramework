using SF.Core.DI;
using System.Collections.Generic;

namespace SF.Core.ManagedServices
{
	public interface IManagedServiceCollection : IDIServiceCollection
	{
		SF.Core.DI.IDIServiceCollection NormalServiceCollection { get; }
	}

}
