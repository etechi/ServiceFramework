
using SF.Clients;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SF.Core.ServiceManagement
{
	
	public static class ClientDIExtensions
	{
		public static IServiceCollection AddLocalClientService(this IServiceCollection sc)
		{
			sc.AddSingleton<IClientService, LocalClientService>();
			return sc;
		}
		
	}
}
