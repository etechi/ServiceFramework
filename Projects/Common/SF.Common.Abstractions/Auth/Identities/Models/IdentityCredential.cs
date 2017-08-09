using System;

namespace SF.Auth.Identities.Models
{
	public class IdentityCredential
	{
		public long ProviderId { get; set; }
		public long IdentityId { get; set; }
		public string Credential { get; set; }
		public string UnionIdent { get; set; }
		public DateTime BindTime { get; set; }
		public DateTime? ConfirmedTime { get; set; }
	}

}

