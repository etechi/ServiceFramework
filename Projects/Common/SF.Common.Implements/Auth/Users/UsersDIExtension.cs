using SF.Core.DI;
using SF.Core.ManagedServices.Admin;

namespace SF.Auth.Identity
{
	public static class UsersDIExtension
	{
		public static IDIServiceCollection UseUsers(
			this IDIServiceCollection sc,
			string TablePrefix = null
			) => UseUsers<DataModels.User, DataModels.UserIdent, Models.MemberInternal,Models.UserEditable,UserQueryArgument>(
				sc,
				TablePrefix);


		public static IDIServiceCollection UseUsers<TUser,TUserIdent,TUserInternal,TUserEditable,TUserQueryArgument>(
			this IDIServiceCollection sc,
			string TablePrefix=null
			)
			where TUser : DataModels.User,new()
			where TUserIdent : DataModels.UserIdent,new()
			where TUserInternal : Models.MemberInternal,new()
			where TUserEditable : Models.UserEditable,new()
		{
			sc.Normal().UseDataModules<
				TUser,
				TUserIdent
				>(TablePrefix);

			sc.AddScoped<IUserService, UserService>();
			sc.AddScoped<IMemberManagementService, EntityUserAdminService<TUser>>();

			
			return sc;
		}
	}
}