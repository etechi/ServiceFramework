﻿#region Apache License Version 2.0
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

using SF.Auth.Users.Internals;
using SF.Core.ServiceManagement;
using System;
using System.Threading.Tasks;

namespace SF.Auth.Users.IdentityCredentialProviders
{
	public class UserAccountIdentityCredentialProvider :
		BaseIdentityCredentialProvider
	{
		public UserAccountIdentityCredentialProvider(
			IUserCredentialStorage IdentStorage,
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
