using SF.System.Auth.Identity.Models;
using System;

namespace SF.System.Auth.Sessions.Models
{
	public class UserSession
    {
        public long Id { get; set; }
		public DateTime? Expires { get; set; }
		public UserDesc User { get; set; }
    }
}

