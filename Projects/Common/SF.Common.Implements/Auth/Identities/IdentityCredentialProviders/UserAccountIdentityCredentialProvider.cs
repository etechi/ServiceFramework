using SF.Data.Storage;
using SF.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using SF.Auth.Identities.Internals;
using SF.KB.PhoneNumbers;
using SF.Common.TextMessages;
using System.Text;
using System.Collections.Generic;
using SF.Core.ServiceManagement;

namespace SF.Auth.Identities.IdentityCredentialProviders
{
	public class UserAccountIdentityCredentialProvider :
		BaseIdentityCredentialProvider
	{
		public UserAccountIdentityCredentialProvider(
			IIdentityCredentialStorage IdentStorage,
			IServiceInstanceDescriptor ServiceInstanceMeta
			) :
			base(IdentStorage,ServiceInstanceMeta)
		{
		}
		public override string Name => "用户账号";

		public override bool IsConfirmable()
		{
			return false;
		}
		
		public override Task<long> SendConfirmCode(
			long? IdentityId, string Ident,  string Code, ConfirmMessageType Type, string TrackIdent)
		{

			throw new NotSupportedException("用户账号不支持验证");
		}

		public override Task<string> VerifyFormat(string Ident)
		{
			if (Ident.Length < 2)
				return Task.FromResult("账号太短");
			return Task.FromResult((string)null);
		}
	}
}
