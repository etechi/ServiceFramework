using SF.Data;
using System;

namespace SF.Auth.Users
{
	public class UserSessionInternal : IObjectWithId<long>
    {
        public long Id { get; set; }
		public long UserId { get; set; }
        public string NickName { get; set; }
        public string Icon { get; set; }
        public string Image { get; set; }
        public SexType? Sex { get; set; }
        public UserType Type { get; set; }

		public DateTime CreatedTime { get; set; }
		public DateTime LastActiveCount { get; set; }

		public string IdentProvider { get; set; }
		public string IdentValue { get; set; }
	}
}

