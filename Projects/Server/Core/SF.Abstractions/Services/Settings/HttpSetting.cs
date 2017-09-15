using SF.Auth;
using SF.Metadata;
using SF.Core.NetworkService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SF.Services.Settings
{
	public class HttpSetting
	{

		[Comment(Name = "主域名", Description = "主域名，格式如： www.abc.com")]
		public string Domain { get; set; }

		[Comment(Name = "启用HTTPS模式", Description = "启用后，访问普通页面的用户将引导至通过HTTPS协议访问")]
		public bool HttpsMode { get; set; }

	}
}
