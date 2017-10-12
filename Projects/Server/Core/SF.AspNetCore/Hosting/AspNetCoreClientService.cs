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


using Microsoft.AspNetCore.Authentication;
using SF.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SF.AspNetCore
{

	public class AspNetCoreClientService : IClientService, IAccessSource
	{
		public Microsoft.AspNetCore.Http.HttpContext Context { get; }
		public IClientDeviceTypeDetector ClientDeviceTypeDetector { get; }
		public AspNetCoreClientService(Microsoft.AspNetCore.Http.IHttpContextAccessor HttpContextAccessor, IClientDeviceTypeDetector ClientDeviceTypeDetector)
		{
			Context = HttpContextAccessor.HttpContext;
			this.ClientDeviceTypeDetector = ClientDeviceTypeDetector;
		}
		public IAccessSource AccessSource => this;

		public long? CurrentScopeId { get; set; }

		IReadOnlyDictionary<string, string> IAccessSource.ExtraValues { get; } = new Dictionary<string, string>();

		string IAccessSource.ClientAddress => Context.Connection.RemoteIpAddress.ToString();

		string IAccessSource.ClientAgent => Context.Request.Headers.UserAgent();

		ClientDeviceType IAccessSource.DeviceType => ClientDeviceTypeDetector.Detect(Context.Request.Headers.UserAgent());

		string _AccessToken;
		public string GetAccessToken()
		{
			if(_AccessToken==null)
				_AccessToken = (from id in Context.User.Identities
						 from c in id.Claims
						 where c.Type == "token"
						 select c.Value).FirstOrDefault();
			return _AccessToken;
		}

		public async Task SetAccessToken(string AccessToken)
		{
			if (AccessToken == null)
				await Context.SignOutAsync("SFAuth");
			else
				await Context.SignInAsync(
					"SFAuth",
					new System.Security.Claims.ClaimsPrincipal(
						EnumerableEx.From(
							new System.Security.Claims.ClaimsIdentity(
								EnumerableEx.From(
									new System.Security.Claims.Claim("token", AccessToken)
								)
							)
						)
					)
				);
		}
	}
}
