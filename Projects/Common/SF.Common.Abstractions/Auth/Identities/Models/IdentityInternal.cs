using System;
using SF.Data;

namespace SF.Auth.Identities.Models
{
	public class IdentityInternal : IObjectWithId<long>
	{
		public long Id { get; set; }
		public string EntityType { get; set; }
		public string Name { get; set; }
		public string Icon { get; set; }
		public string CreateIdent { get; set; }
		public DateTime CreateTime { get; set; }
		public DateTime UpdateTime { get; set; }
	}

}

