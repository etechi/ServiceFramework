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

using SF.Auth.IdentityServices.Internals;
using SF.Common.PhoneNumberValidators;
using SF.Common.Notifications;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SF.Sys;
using SF.Common.Notifications.Management;

namespace SF.Auth.IdentityServices.UserCredentialProviders
{
	public class PhoneNumberUserCredentialProvider :
		IUserCredentialProvider
	{
		public Lazy<IPhoneNumberValidator> PhoneNumberValidator { get; }
		public Lazy<INotificationManager> NotificationManager { get; }
		public PhoneNumberUserCredentialProvider(
			Lazy<IPhoneNumberValidator> PhoneNumberValidator,
			Lazy<INotificationManager> TextMessageService
			)
		{
			this.PhoneNumberValidator = PhoneNumberValidator;
			this.NotificationManager = TextMessageService;
		}
		public long ClaimTypeId => 0;

		public string Name => "手机号";

		public string Ident => "phone";

		public string Description => "手机号认证";

		public bool IsConfirmable()
		{
			return true;
		}
		
		public  async Task<long> SendConfirmCode(
			 long? IdentityId, 
			 string Ident, 
			 string Code, 
			 ConfirmMessageType Type, 
			 string TrackIdent
			)
		{
			var args = new Dictionary<string, object> {
				{"手机号", Ident },
				{"验证码", Code },
				{"业务流水",TrackIdent }
			};
			var re= await NotificationManager.Value.CreateNormalNotification(
				IdentityId,
				Type.ToString(),
				args,
				BizIdent:TrackIdent
				);
			return re;
		}

		public  Task<string> VerifyFormat(string Ident)
		{
			return PhoneNumberValidator.Value.Validate(Ident);
		}
	}
}
