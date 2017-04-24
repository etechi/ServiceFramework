using SF.Metadata;
using SF.Auth;
using SF.Auth.Identities;
using SF.Users.Members.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.Auth.Identities.Models;
using SF.Data.Entity;
using SF.Data.Storage;

namespace SF.Management.SysAdmins
{
	public class SysAdminService :
		ISysAdminService
	{
		SysAdminServiceSetting Setting { get; }
		public SysAdminService(SysAdminServiceSetting Setting) 
		{
			this.Setting = Setting;
		}

		public Task<string> Signin(SigninArgument Arg)
		{
			return Setting.IdentityService.Value.Signin(Arg, Setting.SigninCredentialProviders.Value);
		}

		public Task Signout()
		{
			return Setting.IdentityService.Value.Signout();
		}

		public Task<string> SendPasswordRecorveryCode(SendPasswordRecorveryCodeArgument Arg)
		{
			return Setting.IdentityService.Value.SendPasswordRecorveryCode(Arg, Setting.SignupCredentialProvider.Value);
		}

		public Task<string> ResetPasswordByRecoveryCode(ResetPasswordByRecorveryCodeArgument Arg)
		{
			return Setting.IdentityService.Value.ResetPasswordByRecoveryCode(Arg);
		}

		public Task<string> SetPassword(SetPasswordArgument Arg)
		{
			return Setting.IdentityService.Value.SetPassword(Arg);
		}
	}

}

