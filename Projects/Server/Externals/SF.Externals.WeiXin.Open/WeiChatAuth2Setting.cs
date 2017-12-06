using ServiceProtocol.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProtocol.Auth.Externals.Providers.WeiChat
{

	[Setting(Name = "微信网页授权",Group ="外部接口")]
	public class WeiChatAuth2Setting 
	{
		

		[Display(Name = "公众号APPID", Description = "公众号唯一标识")]
		public string AppId { get; set; } = "wx441c9bf151e90b7b";

		[Display(Name = "公众号APPSecret", Description = "公众号的appsecret")]
		public string AppSecret { get; set; } = "84ac1c235271272dc25d9c636f31055e";

		[Display(Name = "入口页面地址", Description = "微信网页授权入口URL(例:https://open.weixin.qq.com/connect/oauth2/authorize)")]
		public string AuthorizeUri { get; set; } = "https://open.weixin.qq.com/connect/oauth2/authorize";

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
