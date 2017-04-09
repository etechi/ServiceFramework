using SF.Core.DI;
using SF.Core.ManagedServices.Admin;

namespace SF.Auth.Passport
{
	public static class PassportDIExtension
	{
		public static IDIServiceCollection UsePassport(
			this IDIServiceCollection sc,
			string TablePrefix = null
			) => UsePassport<DataModels.UserSession>(
				sc,
				TablePrefix);


		public static IDIServiceCollection UsePassport<TUserSession>(
			this IDIServiceCollection sc,
			string TablePrefix=null
			)
			where TUserSession : DataModels.UserSession,new()
		{
			sc.Normal().UseDataModules<
				TUserSession
				> (TablePrefix);

			sc.AddScoped<IPassportService, PassportService>();
			sc.AddScoped<
				IUserSessionAdminService, 
				EntityUserSessionAdminService<
					TUserSession>
					>();

			
			return sc;
		}
	}
}