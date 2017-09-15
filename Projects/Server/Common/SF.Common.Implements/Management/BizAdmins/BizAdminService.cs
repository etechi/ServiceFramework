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
using SF.Entities;
using SF.Data;

namespace SF.Management.BizAdmins
{
	public class BizAdminService :
		IBizAdminService
	{
		BizAdminServiceSetting Setting { get; }
		public BizAdminService(BizAdminServiceSetting Setting) 
		{
			this.Setting = Setting;
		}

	}

}

