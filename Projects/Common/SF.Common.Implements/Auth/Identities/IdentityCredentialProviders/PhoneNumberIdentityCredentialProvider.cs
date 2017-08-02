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
