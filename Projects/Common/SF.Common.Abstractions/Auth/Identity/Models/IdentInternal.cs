using System;
using SF.Data;

namespace SF.Auth.Identity.Models
{
	public class IdentInternal : IObjectWithId<long>
	{
		public long Id { get; set; }
		public string CreateIdent { get; set; }
		public DateTime CreateTime { get; set; }
		public DateTime UpdateTime { get; set; }
	}

}

