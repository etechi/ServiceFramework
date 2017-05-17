using SF.Auth.Identities.Internals;
using SF.Core.ServiceManagement;
using SF.Core.ServiceManagement.Management;

namespace SF.Auth.Identities.Entity
{
	public static class EntityIdentityDIExtension
	{
		public static IServiceCollection UseIdentityEntityStorage(
			this IServiceCollection sc,
			string TablePrefix = null
			)
		{
			sc.UseDataModules<
				DataModels.Identity,
				DataModels.IdentityCredential
				> (TablePrefix);

			sc.AddScoped<IIdentityManagementService, EntityIdentityManagementService>();
			sc.AddTransient<IIdentStorage>(sp => (IIdentStorage)sp.Resolve<IIdentityManagementService>());
			sc.AddScoped<IIdentityCredentialStorage, EntityIdentityCredentialStorage<DataModels.IdentityCredential>>();
			return sc;
		}
	}
}