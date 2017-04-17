using SF.Data;
using SF.KB;
using System;

namespace SF.Users.Members.Models
{
	public class MemberInternal : MemberDesc
	{
		public string UserName { get; set; }
		public DateTime CreatedTime { get; set; }
		public DateTime? LastSigninTime { get; set; }
		public int SigninCount { get; set; }
		//public UserType Type { get; set; }

	}
}

