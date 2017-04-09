using SF.Data.Storage;
using SF.Data;
using SF.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using SF.Metadata;
using SF.Clients;
using SF.Security;
using SF.Core.Times;
using SF.Auth.Users.Internals;
using SF.Auth.Passport.Internals;

namespace SF.Auth.Users
{
	
	public class UserServiceSetting
	{
		[Comment(Name = "注册需要验证码")]
		public bool SignupVerifyCodeRequired { get; set; } = true;
		public Lazy<IIdentGenerator> IdentGenerator { get; set; }
		public IUserStorage UserStorage { get; set; }
		public Lazy<IAuthSessionProvider> AuthSessionProvider { get; set; }
		public Lazy<IUserIdentProvider> SignupIdentProvider { get; set; }
		public Lazy<IUserIdentProvider[]> SigninIdentProviders { get; set; }
		public Lazy<IClientAccessInfo> AccessInfo { get; set; }
		public Lazy<IPasswordHasher> PasswordHasher { get; set; }
		public Lazy<IDataProtector> DataProtector { get; set; }
		public Lazy<ITimeService> TimeService { get; set; }
	}

}
