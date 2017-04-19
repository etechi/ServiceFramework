using SF.System.Auth.Identity.Models;
using SF.Metadata;
using System.Threading.Tasks;
using SF.Auth;
using SF.System.Auth.Identity;

namespace SF.System.Auth
{
	public class SigninArgument
	{
		public int ScopeId { get; set; }
		public string Ident { get; set; }
		public string Password { get; set; }
		public int? Expires { get; set; }
	}
	
	public class SetPasswordArgument
	{
		public int ScopeId { get; set; }
		public string OldPassword { get; set; }
		public string NewPassword { get; set; }
	}

	public interface IUserService
	{
		Task<UserDesc> Signin(SigninArgument SigninArgument);
		Task Signout(int ScopeId);

		Task<UserDesc> SigninByAccessToken(int ScopeId,string AccessToken,int? Expires);

		[Authorize]
		Task Update(UserDesc Desc);

		[Authorize]
		Task<UserDesc> GetCurIdent(int ScopeId);

	}

}

