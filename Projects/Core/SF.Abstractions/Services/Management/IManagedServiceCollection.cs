using SF.Core.DI;
using System.Collections.Generic;

namespace SF.Services.Management
{
	public interface IManagedServiceCollection : IDIServiceCollection
	{
		SF.Core.DI.IDIServiceCollection NormalServiceCollection { get; }
	}

}
