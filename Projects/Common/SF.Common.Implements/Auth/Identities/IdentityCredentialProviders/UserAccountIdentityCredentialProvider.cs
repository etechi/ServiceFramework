using SF.Auth.Identities.Internals;
using SF.Core.ServiceManagement;
using System;
using System.Threading.Tasks;

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
