using ServiceProtocol.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProtocol.Auth.Externals.Providers.WeiChat
{

	[Setting(Name = "微信网站授权",Group ="外部接口")]
	public class WeiChatSiteAuth2Setting 
	{
		
		[Display(Name = "网站应用APPID", Description = "网站应用APPID")]
		public string AppId { get; set; } = "wxcec91c6d06d685aa";

		[Display(Name = "网站应用APPSecret", Description = "网站应用的appsecret")]
		public string AppSecret { get; set; } = "7b5ddbffd5c73d719c0dc29d507c0667";

		[Display(Name = "入口页面地址", Description = "微信网页授权入口URL(例:https://open.weixin.qq.com/connect/qrconnect)")]
		public string AuthorizeUri { get; set; } = "https://open.weixin.qq.com/connect/qrconnect";

		[Display(Name = "访问令牌请求地址", Description = "微信网页授权访问令牌获取地址(例:https://api.weixin.qq.com/sns/oauth2/access_token)")]
		public string AccessTokenUri { get; set; } = "https://api.weixin.qq.com/sns/oauth2/access_token";

		[Display(Name = "访问令牌刷新地址", Description = "用于刷新用户访问令牌的地址(例:https://api.weixin.qq.com/sns/oauth2/refresh_token)")]
		public string RefreshTokenUri { get; set; } = "https://api.weixin.qq.com/sns/oauth2/refresh_token";

		[Display(Name = "用户信息请求地址", Description = "用于获取微信用户信息的地址(例:https://api.weixin.qq.com/sns/userinfo)")]
		public string UserInfoUri { get; set; } = "https://api.weixin.qq.com/sns/userinfo";

		[Display(Name = "访问令牌验证地址", Description = "用于验证微信用户访问令牌的地址(例:https://api.weixin.qq.com/sns/auth)")]
		public string AccessTokenValidatorUri { get; set; } = "https://api.weixin.qq.com/sns/auth";

	}
}
