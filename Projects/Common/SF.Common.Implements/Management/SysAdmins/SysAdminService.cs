﻿using SF.Metadata;
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

	}

}
