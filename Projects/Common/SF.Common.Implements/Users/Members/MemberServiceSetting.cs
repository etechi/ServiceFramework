using SF.Metadata;
using SF.System.Auth;
using SF.System.Auth.Identity;
using SF.Users.Members.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Users.Members
{
	public class MemberServiceSetting : 
		UserServiceSetting
	{
		public Lazy<IMemberManagementService> ManagementService { get; set; }
	}

}

