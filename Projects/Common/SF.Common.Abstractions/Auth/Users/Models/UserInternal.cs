using SF.Data;
using System;

namespace SF.Auth.Users
{
	public class UserInternal : UserInfo
    {
		public string UserName { get; set; }
		public DateTime CreatedTime { get; set; }
		public DateTime? LastSigninTime { get; set; }
		public int SigninCount { get; set; }
    }
}

