using System.Collections.Generic;
namespace SF.Services.Management
{
	public class ManagedServiceCollection : List<ManagedServiceDescriptor>, IManagedServiceCollection
	{
		public SF.DI.IDIServiceCollection NormalServiceCollection { get; }
		public ManagedServiceCollection(SF.DI.IDIServiceCollection sc)
		{
			NormalServiceCollection = sc;
		}
	}

}
