using SF.Auth.Identities.Internals;
using SF.Core.DI;
using SF.Core.ManagedServices.Admin;

namespace SF.Auth.Identities.Entity
{
	public static class EntityIdentityDIExtension
	{
		public static IDIServiceCollection UseIdentityEntityStorage(
			this IDIServiceCollection sc,
			string TablePrefix = null
			)
		{
			sc.Normal().UseDataModules<
				DataModels.Identity,
				DataModels.IdentityCredential
				> (TablePrefix);

			sc.AddScoped<IIdentityManagementService, EntityIdentityManagementService>();
			sc.Normal().AddTransient<IIdentStorage>(sp => (IIdentStorage)sp.Resolve<IIdentityManagementService>());
			sc.AddScoped<IIdentityCredentialStorage, EntityIdentityCredentialStorage<DataModels.IdentityCredential>>();
			return sc;
		}
	}
}