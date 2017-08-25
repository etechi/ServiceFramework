﻿using System;

namespace SF.Auth.Identities.IdentityCredentialProviders
{
	public class ConfirmMessageTemplateSetting
	{
		public string SigninMessageTemplate { get; set; }
		public string SignupMessageTemplate { get; set; }
		public string PasswordRecorveryMessageTemplate { get; set; }
		public string NormalConfirmMessageTemplate { get; set; }


		public string GetTemplate(ConfirmMessageType Type)
		{
			switch (Type)
			{
				case ConfirmMessageType.Confirm:
					return NormalConfirmMessageTemplate;
				case ConfirmMessageType.PasswordRecorvery:
					return PasswordRecorveryMessageTemplate;
				case ConfirmMessageType.Signin:
					return SigninMessageTemplate;
				case ConfirmMessageType.Signup:
					return SignupMessageTemplate;
				default:
					throw new NotSupportedException();
			}
		}
	}
}
