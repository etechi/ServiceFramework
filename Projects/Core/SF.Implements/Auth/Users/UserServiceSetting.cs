using SF.Data.Entity;
using SF.Data;
using SF.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using SF.Metadata;
using SF.Clients;

namespace SF.Auth.Users
{
	
	public class UserServiceSetting
	{
		[Comment(Name = "注册需要验证码")]
		public bool SignupVerifyCodeRequired { get; set; } = true;
		public IAuthSessionProvider AuthSessionProvider { get; set; }
		public Lazy<IIdentGenerator> IdentGenerator { get; set; }
		public IUserProvider UserProvider { get; set; }
		public IUserIdentProvider SignupIdentProvider { get; set; }
		public IUserIdentProvider[] SigninIdentProviders { get; set; }
		public IClientAccessInfo AccessInfo { get; set; }
		public IPasswordHasher PasswordHasher { get; set; }
	}
}
