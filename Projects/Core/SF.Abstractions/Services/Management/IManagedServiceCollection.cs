using System.Collections.Generic;
namespace SF.Services.Management
{
	public interface IManagedServiceCollection : IList<ManagedServiceDescriptor>
	{
		SF.Core.DI.IDIServiceCollection NormalServiceCollection { get; }
	}

}
