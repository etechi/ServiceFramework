using System;
using System.Collections.Generic;
using SF.Core.DI;
using System.Linq;
namespace SF.Services.Management
{
	public class ManagedServiceCollection : 
		List<ServiceDescriptor>,
		IManagedServiceCollection
	{
		public SF.Core.DI.IDIServiceCollection NormalServiceCollection { get; }
		
		public IEnumerable<Type> ServiceTypes=>
			this.Select(d => d.ServiceType).Distinct();

		public ManagedServiceCollection(SF.Core.DI.IDIServiceCollection sc)
		{
			NormalServiceCollection = sc;
		}

		public void Remove(Type Service)
		{
			for (var i = 0; i < Count; i++)
				if (this[i].ServiceType == Service)
				{
					this.RemoveAt(i);
					i--;
				}
		}
	}

}
