#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0

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
	[Comment("客服设置")]
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
