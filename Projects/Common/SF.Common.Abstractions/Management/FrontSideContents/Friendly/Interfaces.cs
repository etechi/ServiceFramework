using System;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using SF.Management.FrontEndContents.Friendly;
using SF.Metadata;

namespace SF.Management.FrontEndContents
{
	[Comment("PC头部菜单")]
	public interface IPCHeadMenuManager : IImageTextItemGroupManager
	{

	}
	[Comment("PC产品分类菜单")]
	public interface IPCProductCategoryMenuManager : IImageTextItemGroupManager
	{

	}
	[Comment("移动端产品分类菜单")]
	public interface IMobileProductCategoryMenuManager : IImageTextItemGroupManager
	{

	}

	[Comment("PC首页幻灯片")]
	public interface IPCHomeSilderManager : IImageItemGroupManager
	{
	}
	[Comment("移动端首页幻灯片")]
	public interface IMobileHomeSilderManager : IImageItemGroupManager
	{
	}
	[Comment("移动端引导页")]
	public interface IMobileImageLandingPageManager : IImageItemGroupManager
	{
	}
	[Comment("PC尾部菜单")]
	public interface IPCTailMenuManager : ITextGroupTextItemGroupManager
	{

	}
}
