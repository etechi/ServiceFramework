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
	public  class CustomServiceSetting
	{
		[Comment(GroupName = "客服", Name = "客服电话", Description = "显示在网页上的客服电话")]
		[StringLength(20)]
		public string CSPhoneNumber { get; set; }

		[Comment(GroupName = "客服", Name = "在线客服链接", Description = "显示在网页上在线客服链接，一般为在线QQ链接")]
		[StringLength(100)]
		public string CSOnlineService { get; set; }

		[Comment(GroupName = "客服", Name = "QQ客服号", Description = "多个号码以分号分隔")]
		public string CSQQ { get; set; }

		[Comment(GroupName = "客服", Name = "客服微信链接", Description = "显示在网页上的微信公众号")]
		[StringLength(100)]
		public string CSWeichatLink { get; set; }

		[Comment(GroupName = "客服", Name = "新浪微博链接", Description = "显示在网页上的新浪微博号")]
		[StringLength(100)]
		public string CSWeiboLink { get; set; }
	}
}
