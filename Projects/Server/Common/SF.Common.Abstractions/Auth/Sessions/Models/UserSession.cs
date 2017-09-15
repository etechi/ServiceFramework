using SF.Auth.Identities.Models;
using System;

namespace SF.Auth.Sessions.Models
{
	public class UserSession
    {
        public long Id { get; set; }
		public DateTime? Expires { get; set; }
		public Identity Identity { get; set; }
    }
}

