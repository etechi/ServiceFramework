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

using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SF.Sys.Clients
{
	public interface IAccessToken
	{
		ClaimsPrincipal User { get; }
		ClaimsPrincipal Operator { get; }
		IDisposable UseOperator(ClaimsPrincipal NewOperator);
	}
	public interface IDefaultAccessTokenProperties
	{
		string Issuer { get; }
		TimeSpan? Expires { get; }
		//SigningCredentials SigningCredentials { get; }
		//TokenValidationParameters TokenValidationParameters { get; }
	}
	public interface IAccessTokenHandler
	{
		string Create(ClaimsPrincipal User, DateTime? Expires);
		ClaimsPrincipal Validate(string AccessToken);
	}
	public interface IClientService
	{
		IUserAgent UserAgent { get; }
		long? CurrentScopeId { get; }
		Task SignInAsync(ClaimsPrincipal User,DateTime? Expires);
		Task SignOutAsync();

		//string GetAccessToken();
		//Task SetAccessToken(string AccessToken);

		//Task<string> Ensure(AccessInfo ClientInfo,Dictionary<string,string> Items);

		//Task SetItems(string Session,Dictionary<string, string> Values);
		//Task<Dictionary<string, string>> GetItems(string Session);
		//Task ClearItems(string Session,string[] Options);
	}
}