using SF.Data.Storage;
using SF.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using SF.Auth.Users.Internals;

namespace SF.Auth.Users.IdentProviders
{
	public class PhoneNumberUserService :
		BaseIdentProvider
	{
		public PhoneNumberUserService(IUserIdentStorage IdentStorage):base(IdentStorage)
		{
		}
		public override string Name => "手机号";
	
		public override Task<string> SendMessage(string Ident, string Title, string Message, string TrackIdent)
		{
			return null;
		}

		public override Task<string> Verify(string Ident)
		{
			return null;
		}
	}
}
