using SF.Core.DI;
using System.Collections.Generic;

namespace SF.Services.ManagedServices
{
	public interface IManagedServiceCollection : IDIServiceCollection
	{
		SF.Core.DI.IDIServiceCollection NormalServiceCollection { get; }
	}

}
