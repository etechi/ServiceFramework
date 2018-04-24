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

namespace SF.Common.FrontEndContents.Friendly
{
	/// <summary>
	/// 前端内容设置
	/// </summary>
	public class FriendlyContentSetting
	{
		/// <summary>
		/// PC头部菜单
		/// </summary>
		/// <group>PC端</group>
		[EntityIdent(typeof(Content))]
		public long PCHeadMenuId { get; set; }


		/// <summary>
		/// PC头部产品分类菜单
		/// </summary>
		/// <group>PC端</group>
		[EntityIdent(typeof(Content))]
		public long PCHeadProductCategoryMenuId { get; set; }

		/// <summary>
		/// PC首页幻灯片
		/// </summary>
		/// <group>PC端</group>
		[EntityIdent(typeof(Content))]
		public long PCHomePageSliderId { get; set; }

		/// <summary>
		/// PC尾部菜单
		/// </summary>
		/// <group>PC端</group>
		[EntityIdent(typeof(Content))]
		public long PCHomeTailMenuId { get; set; }


		/// <summary>
		/// PC尾部链接
		/// </summary>
		/// <group>PC端</group>
		[EntityIdent(typeof(Content))]
		public long PCHomeTailLinksId { get; set; }

		/// <summary>
		/// PC广告位分类
		/// </summary>
		/// <group>PC端</group>
		public string PCAdCategory { get; set; }

		/// <summary>
		/// 移动端引导页图片
		/// </summary>
		/// <group>移动端</group>
		[EntityIdent(typeof(Content))]
		public long MobileLandingPageImagesId { get; set; }

		/// <summary>
		/// 移动端首页幻灯片
		/// </summary>
		/// <group>移动端</group>
		[EntityIdent(typeof(Content))]
		public long MobileHomePageSliderId { get; set; }

		/// <summary>
		/// 移动端首页链接菜单
		/// </summary>
		/// <group>移动端</group>
		[EntityIdent(typeof(Content))]
		public long MobileHomeIconLinkId { get; set; }

		/// <summary>
		/// 移动端产品分类菜单
		/// </summary>
		/// <group>移动端</group>
		[EntityIdent(typeof(Content))]
		public long MobileProductCategoryMenuId { get; set; }

		/// <summary>
		/// 移动端广告位分类
		/// </summary>
		/// <group>移动端</group>
		public string MobileAdCategory { get; set; }
	}
	
}
