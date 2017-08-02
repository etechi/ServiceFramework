
using SF.Core.ServiceManagement.Management;
using SF.Management.MenuServices;
using System.Linq;
using SF.Metadata;
using SF.Core.NetworkService.Metadata;
using SF.Data.Entity;
using SF.Management.MenuServices.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SF.Core.ServiceManagement
{
	public static class MenuServicesDIExtension
	{
		//class TempItem : SF.Management.MenuServices.Models.MenuItem
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
			//	iis => new SF.Management.MenuServices.Models.MenuItem
			//	{
			//		Children =new List<SF.Management.MenuServices.Models.MenuItem>( iis.Select(i => EntityMapper.Map<TempItem, SF.Management.MenuServices.Models.MenuItem>(i))),
			//		Name = iis[0].Path[iis[0].Path.Length - 1],
			//		Title = iis[0].Path[iis[0].Path.Length - 1]
			//	},
			//	t => t.Path,
			//	(p, n) =>
			//	{
			//		if (p.Children == null)
			//			p.Children = new List<SF.Management.MenuServices.Models.MenuItem>();
			//		((List<SF.Management.MenuServices.Models.MenuItem>)p.Children).Add(n);
			//	}
			//	);
			var items = tree();
			ADT.Tree.AsEnumerable(items, i => i.Children)
				.ForEach(it =>
				{
					if(it.Name==null)
					{
						if (it.Action == MenuItemAction.EntityManager)
							it.Name = it.ActionArgument;
						else
							throw new ArgumentException("菜单项缺少名称:"+Json.Stringify(it));
					}
					it.Title = it.Title ?? it.Name;
				});

			var menuService = sp.Resolve<IMenuService>();
			await menuService.EnsureEntity(
				await menuService.ResolveEntity(new MenuQueryArgument { Ident = "admin" }),
				() => new MenuEditable(),
				e => {
					e.Name = "管理菜单";
					e.Ident = "admin";
					e.Items = items;
				}
			);
		}

		static async Task NewMenu(
			this IServiceProvider sp, 
			long? ParentId, 
			string Ident, 
			string Name, 
			MenuItem[] items
			)
		{
			ADT.Tree.AsEnumerable(items, i => i.Children)
				.ForEach(it =>
				{
					if (it.Name == null)
					{
						if (it.Action == MenuItemAction.EntityManager)
							it.Name = it.ActionArgument;
						else
							throw new ArgumentException("菜单项缺少名称:" + Json.Stringify(it));
					}
					it.Title = it.Title ?? it.Name;
				});

			var menuService = sp.Resolve<IMenuService>();
			await menuService.EnsureEntity(
				await menuService.ResolveEntity(new MenuQueryArgument { Ident = Ident }),
				() => new MenuEditable(),
				e => {
					e.Name = Name;
					e.Ident = Ident;
					e.Items = items;
				}
			);
		}
		public static IServiceCollection UseMenuServices<TMenu, TMenuItem>(
			this IServiceCollection sc,
			//Func<MenuItem[]> DefaultMenu=null,
			string TablePrefix = null
			)
			where TMenu : SF.Management.MenuServices.Entity.DataModels.Menu<TMenu, TMenuItem>,new()
			where TMenuItem: SF.Management.MenuServices.Entity.DataModels.MenuItem<TMenu,TMenuItem>,new()
		{
			sc.AddDataModules<TMenu,TMenuItem>(TablePrefix);
			sc.AddScoped<IMenuService, SF.Management.MenuServices.Entity.EntityMenuService<TMenu, TMenuItem>>();

			//sc.AddInitializer(
			//	"初始化菜单",
			//	sp=>Init(sp,DefaultMenu),
			//	int.MaxValue
			//	);
			return sc;
		}

		public static IServiceCollection UseMenuService(
			this IServiceCollection sc,
			//Func<MenuItem[]> DefaultMenu = null,
			string TablePrefix = null
			)
			=> UseMenuServices<
				SF.Management.MenuServices.Entity.DataModels.Menu, 
				SF.Management.MenuServices.Entity.DataModels.MenuItem
				>(sc, /*DefaultMenu,*/TablePrefix);
	}
}