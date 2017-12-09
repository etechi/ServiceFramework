using System.Threading.Tasks;
using System.Net.Http;
using System.Security.Claims;
using SF.Auth.IdentityServices.Models;
using System.Collections.Generic;

namespace SF.Auth.IdentityServices.Externals
{
	public class ProcessCallbackResult
	{
		public ClaimValue[] Credentials { get; set; }
		public string Token { get; set; }
		public string State { get; set; }
	}


	public class UserInfo
	{
		public ClaimValue[] Claims { get; set; }
		public ClaimValue[] Credentials { get; set; }
	}
	/// <summary>
	/// 外部认证提供者
	/// </summary>
	public interface IExternalAuthorizationProvider
	{
		Task<Dictionary<string, string>> StartClientAuthorization();
		Task<ProcessCallbackResult> ProcessClientCallback(Dictionary<string, string> Argument);

		Task<HttpResponseMessage> StartPageAuthorization(string State, string Callback);
		Task<ProcessCallbackResult> ProcessPageCallback();

		Task<UserInfo> GetUserInfo(string Token);
		Task<string> ValidateAccessToken(string Token);
	}


}
