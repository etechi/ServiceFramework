using System;
using System.Collections.Generic;
using System.Text;

namespace SF.Core.ServiceManagement
{
	public interface IServiceResolverBuilder 
	{	
		IServiceResolver Build(IServiceCollection Services,Caching.ILocalCache<object> AppServiceCache);
	}
}
