
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
using System.Reflection;
using SF.Management.MenuServices.Entity;

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
		public static IServiceInstanceInitializer<IMenuService> NewMenuService(this IServiceInstanceManager manager)
			=>manager.Service<IMenuService, EntityMenuService<SF.Management.MenuServices.Entity.DataModels.Menu, SF.Management.MenuServices.Entity.DataModels.MenuItem>>(null);

		public static async Task NewMenu(
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
			sc.AddManagedScoped<IMenuService, SF.Management.MenuServices.Entity.EntityMenuService<TMenu, TMenuItem>>();

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

		static async Task CollectMenuItem(Models.ServiceInstanceInternal svc,IServiceInstanceManager sim, IServiceDeclarationTypeResolver svcTypeResolver,List<MenuItem> items)
		{
			var type = svcTypeResolver.Resolve(svc.ServiceType);
			var em = type.GetCustomAttribute<EntityManagerAttribute>();
			if (em != null)
				items.Add(new MenuItem
				{
					Name = em.Entity,
					Title = type.Comment().Name,
					Action = MenuItemAction.EntityManager,
					ActionArgument = em.Entity,
					ServiceId=svc.Id
				});
			var re=await sim.QueryAsync(
				new ServiceInstanceQueryArgument
				{
					ParentId = svc.Id
				},Data.Paging.Default
				);
			foreach (var i in re.Items)
				await CollectMenuItem(i, sim, svcTypeResolver, items);
		}
		public static async Task<MenuItem[]> GetServiceMenuItems(this IServiceInstanceManager sim,IServiceProvider sp, long ServiceId)
		{
			var svcTypeResolver = sp.Resolve<IServiceDeclarationTypeResolver>();
			var svc=await sim.GetAsync(ServiceId);
			var items = new List<MenuItem>();
			await CollectMenuItem(svc, sim, svcTypeResolver, items);
			return items.ToArray();
		}
	}
}