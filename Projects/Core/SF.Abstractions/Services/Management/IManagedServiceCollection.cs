using System.Collections.Generic;
namespace SF.Services.Management
{
	public interface IManagedServiceCollection : IList<ManagedServiceDescriptor>
	{
		SF.DI.IDIServiceCollection NormalServiceCollection { get; }
	}

}
