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

using SF.Sys.Annotations;
using System.ComponentModel.DataAnnotations;

namespace SF.Sys.Settings
{
	/// <summary>
	/// 站点设置
	/// </summary>
	public class AppSiteSetting
	{
		///<title>网站名称</title>
		/// <summary>
		/// 用于网站显示的名称
		/// </summary>
		/// <group>基础信息</group>
		[Required]
		[StringLength(20)]
		[Layout(1)]
		public string SiteName { get; set; }

		///<title>网站主图标</title>
		/// <summary>
		/// 用户网站左上角的大图标
		/// </summary>
		/// <group>基础信息</group>
		[Required]
		[Image]
		public string SiteLogo { get; set; }

		///<title>网站小图标</title>
		/// <summary>
		/// 用于二维码生成
		/// </summary>
		/// <group>基础信息</group>
		[Required]
		[Image]
		public string SiteIcon { get; set; }

		///<title>网站版权信息</title>
		/// <summary>
		/// 显示在网站底部的版权信息
		/// </summary>
		/// <group>基础信息</group>
		[Required]
		[StringLength(100)]
		public string SiteCopyright { get; set; }

		///<title>网站认证(ICP证等)</title>
		/// <summary>
		/// 网站ICP/ISP证等
		/// </summary>
		/// <group>基础信息</group>
		[StringLength(100)]
		public string SiteCert { get; set; }
	
	}
}
