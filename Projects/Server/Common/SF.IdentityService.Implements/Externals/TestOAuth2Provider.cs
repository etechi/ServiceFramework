using SF.Auth.IdentityServices.Externals;
using SF.Auth.IdentityServices.Models;
using SF.Sys;
using SF.Sys.Auth;
using SF.Sys.Collections.Generic;
using SF.Sys.Logging;
using SF.Sys.NetworkService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SF.Auth.IdentityServices.Externals
{
	
	/// <summary>
	/// 测试认证
	/// </summary>
	public class TestOAuth2Provider : IOAuthAuthorizationProvider
	{
		public ILogger Logger { get; }
		public IInvokeContext InvokeContext { get; }

		public TestOAuth2Provider( ILogger<TestOAuth2Provider> Logger, IInvokeContext InvokeContext)
		{
			this.Logger = Logger;
			this.InvokeContext = InvokeContext;
		}
		Task<ProcessCallbackResult> GetCallResult(string code, string state)
		{
			return Task.FromResult(new ProcessCallbackResult
			{
				Credentials = new[]
				{
					new ClaimValue(PredefinedClaimTypes.TestId,code),
				},
				Token = code,
				State = state
			});
		}
		public Task<HttpResponseMessage> StartPageAuthorization(string State, string Callback)
		{
			throw new NotSupportedException();
		}

		
		public  Task<ProcessCallbackResult> ProcessPageCallback()
		{
			var reqUri = InvokeContext.Request.Uri;
			var args = new Uri(reqUri).ParseQuery().ToDictionary(p => p.key, p => p.value);
			var code = args.Get("code");
			return  GetCallResult(code,null);
		}

		public Task<string> RefreshToken(string Token)
		{
			return Task.FromResult(Token);
		}
		
		public Task<UserInfo> GetUserInfo(string Token)
		{
			return Task.FromResult(new UserInfo
			{
				Claims =new[]
				{
					new ClaimValue(PredefinedClaimTypes.TestId, Token),
					new ClaimValue(PredefinedClaimTypes.Name,"用户"+Token),
				},
				Credentials = new[]
				{
					new ClaimValue(PredefinedClaimTypes.TestId,Token),
				}
			});
		}
		public  Task<string> ValidateAccessToken(string Token)
		{
			return Task.FromResult((string)null);		
		}

		public Task<Dictionary<string,string>> StartClientAuthorization()
		{
			return Task.FromResult(new Dictionary<string,string>
			{
				{"appid","test" },
				{"scope ", "snsapi_userinfo" },
			});
		}

		public async Task<ProcessCallbackResult> ProcessClientCallback(Dictionary<string, string> Argument)
		{
			if (!Argument.TryGetValue("code", out var code))
				throw new ExternalServiceException($"找不到访问令牌获取码");
			return await GetCallResult(code,null);
		}

	}
}
