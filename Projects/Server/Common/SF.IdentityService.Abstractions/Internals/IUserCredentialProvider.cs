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

using System.Threading.Tasks;
using SF.Auth.IdentityServices.Models;
namespace SF.Auth.IdentityServices.Internals
{
	public enum ConfirmMessageType
	{
		登录,
		注册,
		找回密码,
		验证,
		绑定凭证
	}
	/// <summary>
	/// 用户凭证提供者
	/// </summary>
    public interface IUserCredentialProvider
    {
		string Ident { get; }
		string Name { get; }
		string Description { get; }
		Task<string> VerifyFormat(string Ident);
		Task<long> SendConfirmCode(long? IdentityId, string Credential, string Code, ConfirmMessageType Type, string TrackIdent);
		bool IsConfirmable();
	}

}
