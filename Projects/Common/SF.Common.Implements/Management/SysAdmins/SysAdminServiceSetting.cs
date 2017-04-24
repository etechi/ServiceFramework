using SF.Metadata;
using SF.Auth;
using SF.Auth.Identities;
using SF.Users.Members.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.Data.Storage;

namespace SF.Management.SysAdmins
{
	public class SysAdminServiceSetting 
	{
		public Lazy<IIdentityService> IdentityService { get; set; }
		public Lazy<IIdentityCredentialProvider[]> SigninCredentialProviders { get; set; }
		public Lazy<IIdentityCredentialProvider> SignupCredentialProvider { get; set; }
	}

}

