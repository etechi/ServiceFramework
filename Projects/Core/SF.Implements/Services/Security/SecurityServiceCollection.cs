using SF.Core;
using SF.Core.Caching;
using SF.Metadata;
using SF.Core.Drawing;
using SF.Core.NetworkService;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SF.Services.Security;
using SF.Core.ServiceManagement.Management;

namespace SF.Core.ServiceManagement
{
	public static class SecurityServiceCollectionExtension
	{
		public static IServiceCollection AddDefaultDataProtector(
			this IServiceCollection sc
			)
		{
			sc.AddManagedScoped<IDataProtector, DataProtector>();
			return sc;
		}
		public static IServiceCollection AddDefaultPasswordHasher(
		   this IServiceCollection sc
		   )
		{
			sc.AddManagedScoped<IPasswordHasher, PasswordHasher>();
			return sc;
		}
		public static IServiceCollection AddDefaultSecurityServices(this IServiceCollection sc)
		{
			sc.AddDefaultDataProtector();
			sc.AddDefaultPasswordHasher();
			return sc;
		}
		public static IServiceInstanceInitializer NewDataProtectorService(
			this IServiceInstanceManager manager,
			string GlobalPassword="qw1%^(01e23"
			)
		{
			return manager.DefaultService<IDataProtector, DataProtector>(
				new
				{
					GlobalPassword = GlobalPassword
				}
				);
		}
		public static IServiceInstanceInitializer NewPasswordHasherService(
			this IServiceInstanceManager manager,
			string GlobalPassword = "qw1%^(01e23"
			)
		{
			return manager.DefaultService<IPasswordHasher, PasswordHasher>(
				new
				{
					GlobalPassword = GlobalPassword
				}
				);
		}
	}
}
