using System;
using System.Threading.Tasks;
using System.Net.Http;
using SF.Sys.Auth;
using System.Security.Claims;

namespace SF.Auth.IdentityServices.Externals
{
	public class ProcessCallbackResult
	{
		public Claim[] Credentials { get; set; }
		public string Token { get; set; }
		public string State { get; set; }
	}
	
	public class UserInfo
	{
		public Claim[] Claims { get; set; }
		public Claim[] Credentials { get; set; }
	}
	/// <summary>
	/// 外部认证提供者
	/// </summary>
	public interface IExternalAuthorizationProvider
	{
		Task<HttpResponseMessage> StartAuthorization(string State, string Callback);
		Task<ProcessCallbackResult> ProcessCallback();
		Task<UserInfo> GetUserInfo(string Token);
		Task<string> ValidateAccessToken(string Token);
	}
	public interface IOAuthAuthorizationProvider : IExternalAuthorizationProvider
	{
		Task<string> RefreshToken(string Token);
	}

	public interface IExtAuthService
	{
        //Task<string> GetAuthState(string State);

        Task<HttpResponseMessage> Start(
            string Provider, 
            string Callback
            );
		Task<HttpResponseMessage> Callback(string Provider);
	}

	
	public interface IExternalAuthorizationTokenStorage
	{
		Task Update(string Provider,string UserId,string ExternalUserId, string Token, DateTime Expires);
		Task<string> GetByUserId(string Provider, string UserId);
		Task<string> GetByExternalUserId(string Provider, string ExternalUserId);
	}


}
