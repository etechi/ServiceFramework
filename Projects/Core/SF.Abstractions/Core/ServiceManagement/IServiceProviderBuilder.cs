using SF.Core.ServiceManagement.Internals;
using System;
using System.Collections.Generic;
using System.Text;

namespace SF.Core.ServiceManagement
{
	public interface IServiceProviderBuilder 
	{	
		IServiceProvider Build(
			IServiceCollection Services,
			Caching.ILocalCache<IServiceEntry> AppServiceCache,
			IServiceDeclarationTypeResolver ServiceDeclarationTypeResolver=null,
			IServiceImplementTypeResolver ServiceImplementTypeResolver=null
			);
	}
}
