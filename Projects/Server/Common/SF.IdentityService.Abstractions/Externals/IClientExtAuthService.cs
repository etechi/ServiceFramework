using System.Collections.Generic;
using System.Threading.Tasks;
using SF.Sys.Auth;
using SF.Sys.NetworkService;

namespace SF.Auth.IdentityServices.Externals
{
	public class ExtAuthArgument
	{
		public string Provider { get; set; }
		public string State { get; set; }
		public Dictionary<string, string> Arguments { get; set; }
	}
	/// <summary>
	/// 第三方认证返回参数
	/// </summary>
	public class AuthCallbackArgument
	{
		/// <summary>
		/// 认证状态，来自GetAuthArgument的返回值
		/// </summary>
		public string State { get; set; }

		/// <summary>
		/// 返回参数，由不同提供者决定，比如OAUTH认证使用code
		/// </summary>
		public Dictionary<string, string> Arguments { get; set; }
	}
	public class AuthCallbackResult
	{
		/// <summary>
		/// 访问令牌
		/// </summary>
		public string AccessToken { get; set; }
		/// <summary>
		/// 用户信息
		/// </summary>
		public User User { get; set; }
	}
	[NetworkService]
	[AnonymousAllowed]
	public interface IClientExtAuthService
	{
		/// <summary>
		/// 启动客户端第三方身份认证过程
		/// </summary>
		/// <param name="Provider">外部认证提供者</param>
		/// <param name="ClientId">客户端标识:如app.ios,app.android</param>
		/// <returns>返回第三方身份认证所需参数，具体参数由第三方身份认证提供者决定</returns>
		Task<ExtAuthArgument> GetAuthArgument(string Provider,string ClientId);

		/// <summary>
		/// 第三方身份认证完成
		/// </summary>
		/// <param name="Arg">完成参数</param>
		/// <returns>用户及访问令牌</returns>
		Task<AuthCallbackResult> AuthCallback(AuthCallbackArgument Arg);
	}


}
