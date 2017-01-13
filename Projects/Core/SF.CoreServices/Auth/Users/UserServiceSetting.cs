using ServiceProtocol.Data.Entity;
using SF.Data;
using SF.Data.Entity;
using SF.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Auth.Users
{
	public class UserIdentProviderSetting
	{
		public bool Disabled { get; set; }
		public IUserIdentProvider IdentProvider { get; set; }
	}
	public class UserServiceSetting
	{
		[Display(Name = "注册需要验证码")]
		public bool SignupVerifyCodeRequired { get; set; } = true;
		public IAuthSessionProvider AuthSessionProvider { get; set; }
		public IUserProvider UserProvider { get; set; }
		public UserIdentProviderSetting[] IdentProviders { get; set; }
		public IServiceAccessInfo AccessInfo { get; set; }
		public IPasswordHasher PasswordHasher { get; set; }
	}
}
