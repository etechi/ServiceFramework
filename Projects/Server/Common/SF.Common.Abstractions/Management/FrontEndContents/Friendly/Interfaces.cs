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
