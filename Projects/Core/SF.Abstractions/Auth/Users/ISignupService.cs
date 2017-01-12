using ServiceProtocol;
using SF.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Auth.Users
{
	public class UserSignupArgument 
	{
		public string Icon { get; set; }
		public string Image { get; set; }
		public SexType Sex { get; set; }
		public string NickName { get; set; }
		public string Account { get; set; }
		public string EMail { get; set; }
		public string PhoneNumber { get; set; }
		public string Password { get; set; }
		public string VerifyCode { get; set; }
		public string CaptchaCode { get; set; }
	}
	[NetworkService]
	public interface ISignupService
	{
		Task<UserInfo> Signup(UserSignupArgument Arg);
		Task<string> SendPhoneNumberVerifyCode(string PhoneNumber);
	}
}

