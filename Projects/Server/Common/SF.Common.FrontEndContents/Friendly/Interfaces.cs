﻿#region Apache License Version 2.0
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


namespace SF.Common.FrontEndContents.Friendly
{
	/// <summary>
	/// PC头部菜单
	/// </summary>
	public interface IPCHeadMenuManager : IImageTextItemGroupManager
	{

	}
	/// <summary>
	/// PC产品分类菜单
	/// </summary>
	public interface IPCProductCategoryMenuManager : IImageTextItemGroupManager
	{

	}
	/// <summary>
	/// 移动端产品分类菜单
	/// </summary>
	public interface IMobileProductCategoryMenuManager : IImageTextItemGroupManager
	{

	}

	/// <summary>
	/// PC首页幻灯片
	/// </summary>
	public interface IPCHomeSilderManager : IImageItemGroupManager
	{
	}
	/// <summary>
	/// 移动端首页幻灯片
	/// </summary>
	public interface IMobileHomeSilderManager : IImageItemGroupManager
	{
	}
	/// <summary>
	/// 移动端引导页
	/// </summary>
	public interface IMobileImageLandingPageManager : IImageItemGroupManager
	{
	}
	/// <summary>
	/// PC尾部菜单
	/// </summary>
	public interface IPCTailMenuManager : ITextGroupTextItemGroupManager
	{

	}
}
