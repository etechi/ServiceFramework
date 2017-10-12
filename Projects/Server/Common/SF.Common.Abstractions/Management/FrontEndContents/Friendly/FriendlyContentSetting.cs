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

using System;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using SF.Management.FrontEndContents.Friendly;
using SF.Metadata;

namespace SF.Management.FrontEndContents.Friendly
{
	public class FriendlyContentSetting
	{
		[EntityIdent(typeof(Content))]
		[Comment(Name = "PC头部菜单", GroupName = "PC端")]
		public long PCHeadMenuId { get; set; }

		[EntityIdent(typeof(Content))]
		[Comment(Name = "PC头部产品分类菜单", GroupName = "PC端")]
		public long PCHeadProductCategoryMenuId { get; set; }


		[EntityIdent(typeof(Content))]
		[Comment(Name = "PC首页幻灯片", GroupName = "PC端")]
		public long PCHomePageSliderId { get; set; }

		[EntityIdent(typeof(Content))]
		[Comment(Name = "PC尾部菜单", GroupName = "PC端")]
		public long PCHomeTailMenuId { get; set; }

		[EntityIdent(typeof(Content))]
		[Comment(Name = "PC尾部链接", GroupName = "PC端")]
		public long PCHomeTailLinksId { get; set; }

		[Comment(Name = "PC广告位分类", GroupName = "PC端")]
		public string PCAdCategory { get; set; }


		[EntityIdent(typeof(Content))]
		[Comment(Name = "移动端引导页图片", GroupName = "移动端")]
		public long MobileLandingPageImagesId { get; set; }

		[EntityIdent(typeof(Content))]
		[Comment(Name = "移动端首页幻灯片", GroupName = "移动端")]
		public long MobileHomePageSliderId { get; set; }

		[EntityIdent(typeof(Content))]
		[Comment(Name = "移动端首页链接菜单", GroupName = "移动端")]
		public long MobileHomeIconLinkId { get; set; }

		[EntityIdent(typeof(Content))]
		[Comment(Name = "移动端产品分类菜单", GroupName = "移动端")]
		public long MobileProductCategoryMenuId { get; set; }

		[Comment(Name = "移动端广告位分类", GroupName = "移动端")]
		public string MobileAdCategory { get; set; }
	}
	
}
