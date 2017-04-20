using SF.Data;
using SF.KB;
using System;

namespace SF.Auth.Sessions.Models
{
	public class UserSessionInternal : IObjectWithId<long>
    {
        public long Id { get; set; }
		public long UserId { get; set; }
        public string NickName { get; set; }

		public DateTime CreatedTime { get; set; }
		public DateTime LastActiveCount { get; set; }

		public string IdentProvider { get; set; }
		public string IdentValue { get; set; }
	}
}

