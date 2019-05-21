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
using Microsoft.AspNetCore.Http;
using SF.Sys.Clients;
using SF.Sys.Services;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SF.Sys.AspNetCore
{


	
	public class AspNetCoreClientService : IClientService, IUserAgent, IAccessToken
    {
		public Microsoft.AspNetCore.Http.HttpContext Context { get; }
		public IClientDeviceTypeDetector ClientDeviceTypeDetector { get; }

		public AspNetCoreClientService(IServiceProvider Services, IClientDeviceTypeDetector ClientDeviceTypeDetector)
		{
			Context = Services.Resolve<IHttpContextAccessor>().HttpContext;
			this.ClientDeviceTypeDetector = ClientDeviceTypeDetector;
		}
        public ClaimsPrincipal User
        {
            get
            {
                //Context.ChallengeAsync("Bearer").Wait();
                return (_Users?.Count ?? 0) == 0 ? Operator : _Users.Peek();
            }
        }
        Stack<ClaimsPrincipal> _Users;

        public ClaimsPrincipal Operator => Context?.User;


        public async Task<T> UseUser<T>(ClaimsPrincipal NewUser, Func<Task<T>> Callback)
        {
            if (_Users == null)
                _Users = new Stack<ClaimsPrincipal>();
            _Users.Push(NewUser);
            try
            {
                return await Callback();
            }
            finally
            {
                _Users.Pop();
            }
        }

        public IUserAgent UserAgent => this;
        public IAccessToken AccessToken => this;
        public Uri EntryUri => Context.Request.Uri();
        public long? CurrentScopeId { get; set; }

		IReadOnlyDictionary<string, string> IUserAgent.ExtraValues { get; } = new Dictionary<string, string>();

		string IUserAgent.Address => Context.Connection.RemoteIpAddress.ToString();

		string IUserAgent.AgentName => Context.Request.Headers.UserAgent();

		ClientDeviceType IUserAgent.DeviceType => ClientDeviceTypeDetector.Detect(Context.Request.Headers.UserAgent());

		public Task SignInAsync(ClaimsPrincipal User,DateTime? Expires)
		{
			return Context.SignInAsync(User,new AuthenticationProperties
			{
				ExpiresUtc= Expires.Select(e=>new DateTimeOffset(Expires.Value))
			});
		}
		public Task SignOutAsync()
		{
			return Context.SignOutAsync();
		}

		//public async Task SetAccessToken(string AccessToken)
		//{
		//	if (AccessToken == null)
		//		await Context.SignOutAsync("SFAuth");
		//	else
		//		await Context.SignInAsync(
		//			"admin-bizness",
		//			new System.Security.Claims.ClaimsPrincipal(
		//				EnumerableEx.From(
		//					new System.Security.Claims.ClaimsIdentity(
		//						EnumerableEx.From(
		//							new System.Security.Claims.Claim("token", AccessToken),
		//							new System.Security.Claims.Claim("sid", "123"),
		//							new System.Security.Claims.Claim("name", "aaaaa"),
		//							new System.Security.Claims.Claim(ClaimTypes.Role, "bizadmin")
		//						),
		//						"admin-bizness"
		//					)
		//				)
		//			)
		//		);
		//}
	}
}
