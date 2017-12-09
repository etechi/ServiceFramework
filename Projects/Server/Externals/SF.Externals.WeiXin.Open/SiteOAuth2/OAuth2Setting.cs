using System.ComponentModel.DataAnnotations;

namespace SF.Externals.WeiXin.Open.SiteOAuth2
{

	/// <summary>
	/// 微信网站授权
	/// </summary>
	public class OAuth2Setting 
	{
		/// <summary>
		/// 网站应用APPID
		/// </summary>
		public string AppId { get; set; }

		/// <summary>
		/// 网站应用APPSecret
		/// </summary>
		public string AppSecret { get; set; }

		///<title>入口页面地址</title>
		/// <summary>
		/// 微信网页授权入口URL(例:https://open.weixin.qq.com/connect/qrconnect)
		/// </summary>
		[Required]
		public string AuthorizeUri { get; set; } = "https://open.weixin.qq.com/connect/qrconnect";

		///<title>访问令牌请求地址</title>
		/// <summary>
		/// 微信网页授权访问令牌获取地址(例:https://api.weixin.qq.com/sns/oauth2/access_token)
		/// </summary>
		[Required]
		public string AccessTokenUri { get; set; } = "https://api.weixin.qq.com/sns/oauth2/access_token";

		///<title>访问令牌刷新地址</title>
		/// <summary>
		/// 用于刷新用户访问令牌的地址(例:https://api.weixin.qq.com/sns/oauth2/refresh_token)
		/// </summary>
		[Required]
		public string RefreshTokenUri { get; set; } = "https://api.weixin.qq.com/sns/oauth2/refresh_token";

		///<title>用户信息请求地址</title>
		/// <summary>
		/// 用于获取微信用户信息的地址(例:https://api.weixin.qq.com/sns/userinfo)
		/// </summary>
		[Required]
		public string UserInfoUri { get; set; } = "https://api.weixin.qq.com/sns/userinfo";

		///<title>访问令牌验证地址</title>
		/// <summary>
		/// 用于验证微信用户访问令牌的地址(例:https://api.weixin.qq.com/sns/auth)
		/// </summary>
		[Required]
		public string AccessTokenValidatorUri { get; set; } = "https://api.weixin.qq.com/sns/auth";



	}
}
