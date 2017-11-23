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

using System.ComponentModel.DataAnnotations;

namespace SF.Sys.Settings
{
	/// <summary>
	/// 客服设置
	/// </summary>
	public class CustomServiceSetting
	{
		///<title>客服电话</title>
		/// <summary>
		/// 显示在网页上的客服电话
		/// </summary>
		/// <group>客服</group>
		[StringLength(20)]
		public string CSPhoneNumber { get; set; }

		///<title>在线客服链接</title>
		/// <summary>
		/// 显示在网页上在线客服链接，一般为在线QQ链接
		/// </summary>
		/// <group>基础信息</group>
		[StringLength(100)]
		public string CSOnlineService { get; set; }

		///<title>QQ客服号</title>
		/// <summary>
		/// 多个号码以分号分隔
		/// </summary>
		/// <group>客服</group>
		public string CSQQ { get; set; }

		///<title>客服微信链接</title>
		/// <summary>
		/// 显示在网页上的微信公众号
		/// </summary>
		/// <group>客服</group>
		[StringLength(100)]
		public string CSWeichatLink { get; set; }

		///<title>新浪微博链接</title>
		/// <summary>
		/// 显示在网页上的新浪微博号
		/// </summary>
		/// <group>客服</group>
		[StringLength(100)]
		public string CSWeiboLink { get; set; }
	}
}
