using SF.Data;
using SF.KB;
using System;

namespace SF.Management.SysAdmins.Models
{
	public class SysAdminInternal : SysAdminDesc
	{
		public string UserName { get; set; }
		public DateTime CreatedTime { get; set; }
		public DateTime? LastSigninTime { get; set; }
		public int SigninCount { get; set; }
		//public UserType Type { get; set; }

	}
}

