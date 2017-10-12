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
	[Comment("站点设置")]
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
