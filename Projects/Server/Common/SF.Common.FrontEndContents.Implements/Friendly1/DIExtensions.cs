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


using SF.Common.FrontEndContents.Friendly;
using SF.Sys.Settings;

namespace SF.Sys.Services
{
	public static class FriendlyFrontEndServicesDIExtensions
	{
		public static IServiceCollection AddFriendlyFrontEndServices(this IServiceCollection sc)
		{

			sc.AddSetting<FriendlyContentSetting>();


			//sc.EntityServices(
			//	"FriendlyFontEndContent",
			//	"定制前端设置",
			//	d => d.Add<IMobileHomeSilderManager, MobileHomeSilderManager>("MobileHomeSilder", "移动端首页幻灯片", false)
			//		.Add<IMobileImageLandingPageManager, MobileImageLandingPageManager>("MobileImageLanding", "移动端引导页", false)
			//		.Add<IMobileProductCategoryMenuManager, MobileProductCategoryMenuManager>("MobileProductCategory", "移动端产品目录", false)
			//		.Add<IPCHeadMenuManager, PCHeadMenuManager>("PCHeadMenu", "PC头部菜单", false)
			//		.Add<IPCTailMenuManager, PCTailMenuManager>("PCTailMenu", "PC尾部菜单", false)
			//		.Add<IPCHomeSilderManager, PCHomeSilderManager>("PCHomeSilder", "PC首页幻灯片", false)
			//		.Add<IPCProductCategoryMenuManager, PCProductCategoryMenuManager>("PCProductCategory", "PC产品目录", false)
			//		.Add<IPCAdManager, PCAdManager>("PCAd", "PC广告", false)
			//		.Add<IMobileAdManager, MobileAdManager>("MobileAd", "移动端广告", false)
			//	);

			//sc.AddDefaultMenuItems(
			//	"bizness",
			//	"内容管理",
			//	new SF.Management.MenuServices.MenuItem{Name = "移动端首页幻灯片",Action = MenuActionType.EntityManager,ActionArgument = "MobileHomeSilder"},
			//	new SF.Management.MenuServices.MenuItem{Name = "移动端引导页",Action = MenuActionType.EntityManager,ActionArgument = "MobileImageLanding"},
			//	new SF.Management.MenuServices.MenuItem{Name = "移动端产品目录",Action = MenuActionType.EntityManager,ActionArgument = "MobileProductCategory"},
			//	new SF.Management.MenuServices.MenuItem { Name = "PC头部菜单", Action = MenuActionType.EntityManager, ActionArgument = "PCHeadMenu" },
			//	new SF.Management.MenuServices.MenuItem{Name = "PC尾部菜单",Action = MenuActionType.EntityManager,ActionArgument = "PCTailMenu" },
			//	new SF.Management.MenuServices.MenuItem{Name = "PC首页幻灯片",Action = MenuActionType.EntityManager,ActionArgument = "PCHomeSilder"},
			//	new SF.Management.MenuServices.MenuItem{Name = "PC产品目录",Action = MenuActionType.EntityManager,ActionArgument = "PCProductCategory"},
			//	new SF.Management.MenuServices.MenuItem { Name = "PC广告", Action = MenuActionType.EntityManager, ActionArgument = "PCAd" },
			//	new SF.Management.MenuServices.MenuItem { Name = "移动端广告", Action = MenuActionType.EntityManager, ActionArgument = "MobileAd" }

			//	);
			return sc;
		}
	}
}
