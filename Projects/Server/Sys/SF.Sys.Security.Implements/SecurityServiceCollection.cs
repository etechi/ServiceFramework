#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0

using SF.Services.Security;
using SF.Sys.Security;
using SF.Sys.Services.Management;

namespace SF.Sys.Services
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
		public static IServiceCollection AddDefaultCaptchaImageService(
			this IServiceCollection sc
			)
		{
			sc.AddManagedScoped<ICaptchaImageService, CaptchaImageService>();
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
			sc.AddDefaultCaptchaImageService();
			return sc;
		}
		public static IServiceInstanceInitializer<IDataProtector> NewDataProtectorService(
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
		public static IServiceInstanceInitializer<IPasswordHasher> NewPasswordHasherService(
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
		public static IServiceInstanceInitializer<ICaptchaImageService> NewCaptchaImageService(
		   this IServiceInstanceManager manager,
		   int CharCount=6,
		   int ExpireMinutes=3
		   )
		{
			return manager.DefaultService<ICaptchaImageService, CaptchaImageService>(
				new
				{
					Setting=new CaptchaImageSetting
					{
						CodeChars=CharCount,
						Expires=ExpireMinutes
					}
				}
				);
		}
	}
}
