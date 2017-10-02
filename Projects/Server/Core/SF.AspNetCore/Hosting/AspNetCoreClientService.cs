
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
