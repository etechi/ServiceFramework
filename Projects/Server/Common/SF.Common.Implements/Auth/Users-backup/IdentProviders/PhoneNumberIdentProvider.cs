using SF.Data.Storage;
using SF.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using SF.Auth.Identity.Internals;

namespace SF.Auth.Identity.IdentProviders
{
	public class PhoneNumberUserService :
		BaseIdentProvider
	{
		public PhoneNumberUserService(IIdentBindStorage IdentStorage):base(IdentStorage)
		{
		}
		public override string Name => "手机号";
	
		public override Task<string> SendConfirmCode(string Ident, string Title, string Message, string TrackIdent)
		{
			return null;
		}

		public override Task<string> VerifyFormat(string Ident)
		{
			return null;
		}
	}
}
