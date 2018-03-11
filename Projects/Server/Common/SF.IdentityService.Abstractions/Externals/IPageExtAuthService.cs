using System.Threading.Tasks;
using System.Net.Http;
using SF.Sys.NetworkService;
using SF.Sys.Auth;

namespace SF.Auth.IdentityServices.Externals
{
	[NetworkService]
	[AnonymousAllowed]
	public interface IPageExtAuthService
	{
		/// <summary>
		/// 开始页面登陆
		/// </summary>
		/// <remarks>
		///		使用window.location或链接引导用户进入此链接。
		///		登录成功后，跳转至Callback链接，并在fragment中附带access_token
		///	</remarks>
		/// <param name="Provider">登录提供者</param>
		/// <param name="Callback">登录完以后跳转链接</param>
		/// <param name="ClientId">客户端ID</param>
		/// <param name="State">登录状态</param>
		/// <returns>跳转至第三方登录页面</returns>
       Task<HttpResponseMessage> Start(
            string Provider, 
            string Callback,
			string ClientId,
			string State
            );

		/// <summary>
		/// 供第三方登录提供者回调使用
		/// </summary>
		/// <param name="Id"></param>
		/// <returns></returns>
		Task<HttpResponseMessage> Callback(string Id);
	}


}
