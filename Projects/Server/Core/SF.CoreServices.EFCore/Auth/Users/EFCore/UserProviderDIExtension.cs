using SF.DI;

namespace SF.Services.Management
{
	public static class UserProviderDIExtension
	{
		public static Services.Management.IManagedServiceCollection UseEFCoreUser<TUser>(
			this Services.Management.IManagedServiceCollection msc,
			string TablePrefix
			)
			where TUser: SF.Auth.Users.EFCore.DataModels.User
		{
			msc.NormalServiceCollection.UseDataModules<TUser>(TablePrefix);
			msc.NormalServiceCollection.UseDataModules<SF.Auth.Users.EFCore.DataModels.UserPhoneNumberIdent>(TablePrefix);
			msc.AddScoped<SF.Auth.Users.IUserProvider, SF.Auth.Users.EFCore.UserProvider<TUser>>();
			return msc;
		}
		public static Services.Management.IManagedServiceCollection UseEFCoreUser(
		   this Services.Management.IManagedServiceCollection msc,
		   string TablePrefix
		   )
		{
			return msc.UseEFCoreUser<SF.Auth.Users.EFCore.DataModels.User>(TablePrefix);
		}
	}
}