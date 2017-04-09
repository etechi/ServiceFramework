using SF.Data;
using SF.KB;
using System;

namespace SF.Auth.Users.Models
{
	public class UserInternal : UserInfo
    {
		public string UserName { get; set; }
		public DateTime CreatedTime { get; set; }
		public DateTime? LastSigninTime { get; set; }
		public int SigninCount { get; set; }
		//public UserType Type { get; set; }

	}
}

