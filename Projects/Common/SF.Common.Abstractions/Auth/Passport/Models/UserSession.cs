using System;

namespace SF.Auth.Passport.Models
{
	public class UserSession
    {
        public long Id { get; set; }
		public DateTime? Expires { get; set; }
		public UserDesc User { get; set; }
    }
}

