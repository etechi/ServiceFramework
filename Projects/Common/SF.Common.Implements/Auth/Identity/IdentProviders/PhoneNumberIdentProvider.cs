using SF.Data.Storage;
using SF.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using SF.Auth.Identity.Internals;
using SF.KB.PhoneNumbers;
using SF.System.TextMessages;
using System.Text;
using System.Collections.Generic;

namespace SF.Auth.Identity.IdentProviders
{
	public class PhoneNumberUserService :
		BaseIdentProvider
	{
		public Lazy<IPhoneNumberValidator> PhoneNumberValidator { get; }
		public Lazy<ITextMessageService> TextMessageService { get; }
		public ConfirmMessageTemplateSetting ConfirmMessageSetting { get; }
		public PhoneNumberUserService(
			IIdentBindStorage IdentStorage,
			Lazy<IPhoneNumberValidator> PhoneNumberValidator,
			Lazy<ITextMessageService> TextMessageService,
			ConfirmMessageTemplateSetting ConfirmMessageSetting
			) :
			base(IdentStorage)
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
			int ScopeId, string Ident, string Code, ConfirmMessageType Type, string TrackIdent)
		{

			var args = new Dictionary<string, string> {
				{"手机号", Ident },
				{"验证码", Code },
				{"业务序号",TrackIdent }
			};
			var re= await TextMessageService.Value.Send(
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
