using SF.Auth.Identity.Internals;
using SF.Core.DI;
using SF.Core.ManagedServices.Admin;

namespace SF.Auth.Identity.Entity
{
	public static class EntityIdentityDIExtension
	{
		public static IDIServiceCollection UseIdentityEntityStorage(
			this IDIServiceCollection sc,
			string TablePrefix = null
			)
		{
			sc.Normal().UseDataModules<
				DataModels.Ident,
				DataModels.IdentBind
				> (TablePrefix);

			sc.AddScoped<IIdentManagementService, EntityIdentManagementService>();
			sc.Normal().AddTransient<IIdentStorage>(sp => (IIdentStorage)sp.Resolve<IIdentManagementService>());
			sc.AddScoped<IIdentBindStorage, EntityIdentBindStorage<DataModels.IdentBind>>();
			return sc;
		}
	}
}