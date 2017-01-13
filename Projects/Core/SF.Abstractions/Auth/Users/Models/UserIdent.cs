using System;

namespace SF.Auth.Users
{
	public class UserIdent
	{
		public string ProviderId { get; set; }
		public string Ident { get; set; }
		public string UnionIdent { get; set; }
		public DateTime BindTime { get; set; }
		public DateTime? ConfirmedTime { get; set; }
	}

}

