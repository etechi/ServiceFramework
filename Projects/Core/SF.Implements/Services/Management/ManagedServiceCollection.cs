using System.Collections.Generic;
namespace SF.Services.Management
{
	public class ManagedServiceCollection : List<ManagedServiceDescriptor>, IManagedServiceCollection
	{
		public SF.Core.DI.IDIServiceCollection NormalServiceCollection { get; }
		public ManagedServiceCollection(SF.Core.DI.IDIServiceCollection sc)
		{
			NormalServiceCollection = sc;
		}
	}

}
