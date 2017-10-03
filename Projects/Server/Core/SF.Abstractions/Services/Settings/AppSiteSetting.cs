using SF.Auth;
using SF.Metadata;
using SF.Core.NetworkService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SF.Services.Settings
{
	public  class AppSiteSetting
	{
		[Comment(GroupName = "基础信息", Name = "网站名称", Description = "用于网站显示的名称")]
		[Required]
		[StringLength(20)]
		[Layout(1)]
		public string SiteName { get; set; }

		[Comment(GroupName = "基础信息", Name = "网站主图标", Description = "用户网站左上角的大图标")]
		[Required]
		[Image]
		public string SiteLogo { get; set; }

		[Comment(GroupName = "基础信息", Name = "网站小图标", Description = "用于二维码生成")]
		[Required]
		[Image]
		public string SiteIcon { get; set; }

		[Comment(GroupName = "基础信息", Name = "网站版权信息", Description = "显示在网站底部的版权信息")]
		[Required]
		[StringLength(100)]
		public string SiteCopyright { get; set; }

		[Comment(GroupName = "基础信息", Name = "网站认证(ICP证等)", Description = "网站ICP/ISP证等")]
		[StringLength(100)]
		public string SiteCert { get; set; }
	
	}
}
