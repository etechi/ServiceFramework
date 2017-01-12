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
	public class SetPasswordArgument
	{
		public string OldPassword { get; set; }
		public string NewPassword { get; set; }
	}
	public class SendPasswordRecorveryCodeArgument
	{
		public string IdentVerifyServiceId { get; set; }
		public string Ident { get; set; }
	}
	public class ResePasswordByRecorveryCodeArgument
	{
		public string IdentVerifyServiceId { get; set; }
		public string Ident { get; set; }
		public string Code { get; set; }
		public string NewPassword { get; set; }
	}
	[NetworkService]
	public interface IUserPasswordService
    {
		[Authorize]
		Task SetPassword(SetPasswordArgument Arg);

		Task<string> SendPasswordRecorveryCode(SendPasswordRecorveryCodeArgument Arg);
		Task ResertPasswordByRecoveryCode(ResePasswordByRecorveryCodeArgument Arg);
    }
   
}
