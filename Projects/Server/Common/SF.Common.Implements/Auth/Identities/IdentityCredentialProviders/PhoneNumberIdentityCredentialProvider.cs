#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0

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
