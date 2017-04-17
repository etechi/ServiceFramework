using System;

namespace SF.System.Auth.Identity.Models
{
	public class IdentBind
	{
		public long UserId { get; set; }
		public string Ident { get; set; }
		public string UnionIdent { get; set; }
		public DateTime BindTime { get; set; }
		public DateTime? ConfirmedTime { get; set; }
	}

}

