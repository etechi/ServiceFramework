using SF.Auth.Identities.Internals;
using SF.Common.TextMessages;
using SF.Core.ServiceManagement;
using SF.KB.PhoneNumbers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SF.Auth.Identities.IdentityCredentialProviders
{
	public class PhoneNumberIdentityCredentialProvider :
		BaseIdentityCredentialProvider
	{
		public Lazy<IPhoneNumberValidator> PhoneNumberValidator { get; }
		public Lazy<IPhoneMessageService> TextMessageService { get; }
		public ConfirmMessageTemplateSetting ConfirmMessageSetting { get; }
		public PhoneNumberIdentityCredentialProvider(
			IIdentityCredentialStorage IdentStorage,
			Lazy<IPhoneNumberValidator> PhoneNumberValidator,
			Lazy<IPhoneMessageService> TextMessageService,
			ConfirmMessageTemplateSetting ConfirmMessageSetting,
			IServiceInstanceDescriptor ServiceInstanceMeta
			) :
			base(IdentStorage,ServiceInstanceMeta)
		{
			this.PhoneNumberValidator = PhoneNumberValidator;
			this.ConfirmMessageSetting = ConfirmMessageSetting;
			this.TextMessageService = TextMessageService;
		}
		public override string Name => "手机号";

		public override bool IsConfirmable()
		{
			return true;
		}
		
		public override async Task<long> SendConfirmCode(
			 long? IdentityId, string Ident, string Code, ConfirmMessageType Type, string TrackIdent)
		{

			var args = new Dictionary<string, string> {
				{"手机号", Ident },
				{"验证码", Code },
				{"业务序号",TrackIdent }
			};
			var re= await TextMessageService.Value.Send(
				IdentityId,
				Ident,
				new Message
				{
					Body=SimpleTemplate.Eval(
						ConfirmMessageSetting.GetTemplate(Type),
						args
					),
					Arguments=args,
					TrackEntityId=TrackIdent
				}
				);
			return re;
		}

		public override Task<string> VerifyFormat(string Ident)
		{
			return PhoneNumberValidator.Value.Validate(Ident);
		}
	}
}
