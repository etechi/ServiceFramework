using SF.System.Auth.Identity.Models;
using System;

namespace SF.System.Auth.Passport.Models
{
	public class UserSession
    {
        public long Id { get; set; }
		public DateTime? Expires { get; set; }
		public IdentDesc User { get; set; }
    }
}

