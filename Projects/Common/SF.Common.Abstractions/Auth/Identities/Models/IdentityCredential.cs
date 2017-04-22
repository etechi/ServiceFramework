using System;

namespace SF.Auth.Identities.Models
{
	public class IdentityCredential
	{
		public long UserId { get; set; }
		public string Ident { get; set; }
		public string UnionIdent { get; set; }
		public DateTime BindTime { get; set; }
		public DateTime? ConfirmedTime { get; set; }
	}

}

