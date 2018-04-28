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


using SF.Sys.BackEndConsole;
using System.Linq;
using SF.Sys.BackEndConsole.Models;
using System;
using System.Threading.Tasks;
using SF.Sys.BackEndConsole.Entity;
using SF.Sys.ADT;
using SF.Sys.Services;
using SF.Sys.Entities;
using SF.Sys.Services.Management;
using SF.Sys.Linq;
using SF.Sys;

namespace SF.Sys.Services
{
	public static class MenuServicesDIExtension
	{
		//class TempItem : SF.Sys.MenuServices.Models.MenuItem
		//{
		//	public string[] Path { get; set; }
		//}
		static async Task Init(IServiceProvider sp,Func<MenuItem[]> tree)
		{
			//var lib = sp.Resolve<Library>();
			//var items = new List<TempItem>();

			//items.AddRange(
			//	from svc in lib.Services
			//	let ems = svc.Attributes.FirstOrDefault(a => a.Type == typeof(EntityManagerAttribute).FullName)?.Values
			//	where ems != null
			//	let em = Json.Parse<EntityManagerAttribute>(ems)
			//	select new TempItem
			//	{
			//		Path = svc.Categories,
			//		Action = MenuItemAction.EntityManager,
			//		ActionArgument = em.Entity,
			//		Name = svc.Name,
			//		Title = svc.Title,
			//		Description = svc.Description,
			//		Remarks = svc.Prompt,
			//		FontIcon = em.FontIcon
			//	}
			//);

			//var tree = items.BuildTreeByParentPath(
			//	iis => new SF.Sys.MenuServices.Models.MenuItem
			//	{
			//		Children =new List<SF.Sys.MenuServices.Models.MenuItem>( iis.Select(i => EntityMapper.Map<TempItem, SF.Sys.MenuServices.Models.MenuItem>(i))),
			//		Name = iis[0].Path[iis[0].Path.Length - 1],
			//		Title = iis[0].Path[iis[0].Path.Length - 1]
			//	},
			//	t => t.Path,
			//	(p, n) =>
			//	{
			//		if (p.Children == null)
			//			p.Children = new List<SF.Sys.MenuServices.Models.MenuItem>();
			//		((List<SF.Sys.MenuServices.Models.MenuItem>)p.Children).Add(n);
			//	}
			//	);
			var items = tree();
			Tree.AsEnumerable(items, i => i.Children)
				.ForEach(it =>
				{
					if(it.Name==null)
					{
						if (it.Action == MenuActionType.EntityManager)
							it.Name = it.ActionArgument;
						else
							throw new ArgumentException("菜单项缺少名称:"+Json.Stringify(it));
					}
				});

			var menuService = sp.Resolve<IMenuService>();
			await menuService.EnsureEntity(
				await menuService.QuerySingleEntityIdent(new MenuQueryArgument { Ident = "admin" }),
				() => new MenuEditable(),
				e => {
					e.Name = "管理菜单";
					e.Ident = "admin";
					e.Items = items;
				}
			);
		}
		public static IServiceInstanceInitializer<IMenuService> NewMenuService(this IServiceInstanceManager manager)
			=>manager.Service<IMenuService, EntityMenuService>(null)
			.WithConsolePages(
				"系统管理/菜单管理"
				);

		public static async Task NewMenu(
			this IServiceProvider sp, 
			long? ParentId, 
			string Ident, 
			string Name, 
			MenuItem[] items
			)
		{
			Tree.AsEnumerable(items, i => i.Children)
				.ForEach(it =>
				{
					if (it.Name == null)
					{
						if (it.Action == MenuActionType.EntityManager)
							it.Name = it.ActionArgument;
						else
							throw new ArgumentException("菜单项缺少名称:" + Json.Stringify(it));
					}
				});

			var menuService = sp.Resolve<IMenuService>();
			await menuService.EnsureEntity(
				await menuService.QuerySingleEntityIdent(new MenuQueryArgument { Ident = Ident }),
				() => new MenuEditable(),
				e => {
					e.Name = Name;
					e.Ident = Ident;
					e.Items = items;
				}
			);
		}
		public static IServiceCollection AddMenuServices(
			this IServiceCollection sc,
			//Func<MenuItem[]> DefaultMenu=null,
			string TablePrefix = null
			)
		{
			sc.AddSingleton<IDefaultMenuCollection, DefaultMenuCollection>();
			sc.AddDataModules<BackEndConsole.Entity.DataModels.DataMenu>(TablePrefix??"Sys");
			sc.EntityServices(
				"SysMenu",
				"系统菜单",
				d => d.Add<IMenuService, SF.Sys.BackEndConsole.Entity.EntityMenuService>("SysMenu","系统菜单")
				);

			sc.AddInitializer(
				"service",
				"初始化默认菜单",
				sp => InitDefaultMenu(sp),
				int.MaxValue
				);
			return sc;
		}

		static async Task InitDefaultMenu(IServiceProvider sp)
		{
			var dmc = sp.Resolve<IDefaultMenuCollection>();
			var mm = sp.Resolve<IMenuService>();

			foreach (var key in dmc.MenuIdents)
				await mm.EnsureEntity(
					await mm.QuerySingleEntityIdent(new MenuQueryArgument { Ident = key }),
					()=>new MenuEditable { Ident=key, Name=key},
					me => {
						me.Items = dmc.GetMenuItems(key);
						});
		}

	
		
	}
}