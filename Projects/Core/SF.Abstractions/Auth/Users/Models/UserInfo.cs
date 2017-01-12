﻿namespace SF.Auth.Users
{
	public class UserInfo
    {
        public long Id { get; set; }
        public string NickName { get; set; }
        public string Icon { get; set; }
        public string Image { get; set; }
        public SexType Sex { get; set; }
        public UserType Type { get; set; }
    }
}

