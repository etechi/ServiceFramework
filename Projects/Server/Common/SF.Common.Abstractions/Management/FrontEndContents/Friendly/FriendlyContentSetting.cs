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
