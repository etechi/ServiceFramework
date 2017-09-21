using SF.Auth.Identities;
using SF.Core.Times;
using SF.Data;
using SF.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
using SF.Management.MenuServices.Models;
using System.Collections.Generic;
using SF.Core.ServiceManagement;

namespace SF.Management.MenuServices.Entity
{
	public class EntityMenuService<TMenu,TMenuItem> :
		EntityManager<long, Models.Menu,  MenuQueryArgument, Models.MenuEditable, TMenu>,
		IMenuService
		where TMenu: DataModels.Menu<TMenu,TMenuItem>,new()
		where TMenuItem : DataModels.MenuItem<TMenu, TMenuItem>, new()
	{
		public Lazy<IDataSet<TMenuItem>> MenuItemSet { get; }
		
		public EntityMenuService(
			IDataSetEntityManager<TMenu> Manager,
			Lazy<IDataSet<TMenuItem>> MenuItemSet
			) : base(Manager)
		{
			this.MenuItemSet = MenuItemSet;
		}

		protected override PagingQueryBuilder<TMenu> PagingQueryBuilder =>
			PagingQueryBuilder<TMenu>.Simple("ident", b => b.Ident, true);
		protected override async Task<MenuEditable> OnMapModelToEditable(IContextQueryable<TMenu> Query)
		{
			var menu = await Query.SelectObjectEntity(i =>
				  new MenuEditable
				  {
					  Id=i.Id,
					  Ident=i.Ident
				  }).SingleOrDefaultAsync();
			if (menu == null)
				return null;

			var items = await MenuItemSet.Value.AsQueryable()
				.Where(i => i.MenuId == menu.Id)
				.SelectUIObjectEntity(i =>
				  new MenuItem
				  {
					  Id = i.Id,
					  Action = i.Action,
					  ActionArgument = i.ActionArgument,
					  ServiceId=i.ServiceId,
					  ParentId=i.ParentId
				  }).ToArrayAsync();
			menu.Items = ADT.Tree.Build(
				items,
				i =>i.Id,
				i=>i.ParentId??0,
				(p,i)=> {
					if (p.Children == null)
						p.Children = new List<MenuItem>();
					((List<MenuItem>)p.Children).Add(i);
				}
				);
			return menu;
		}
		protected override IContextQueryable<Menu> OnMapModelToDetail(IContextQueryable<TMenu> Query)
		{
			return Query.SelectObjectEntity(m => new Menu
			{
				Id = m.Id,
				Ident = m.Ident
			});
		}
		protected override IContextQueryable<TMenu> OnBuildQuery(IContextQueryable<TMenu> Query, MenuQueryArgument Arg, Paging paging)
		{
			var scopeid = ServiceInstanceDescriptor.ParentInstanceId;
			var q = Query.Where(m=>m.ScopeId==scopeid)
				.Filter(Arg.Id, r => r.Id)
				.FilterContains(Arg.Name, r => r.Name)
				.Filter(Arg.Ident, r => r.Ident)
				;
			
			return q;
		}

		protected override async Task OnNewModel(IModifyContext ctx)
		{
			var m = ctx.Model;
			m.Id = await IdentGenerator.GenerateAsync("系统菜单",0);
			m.Create(Now);
			await base.OnNewModel(ctx);
		}
		protected override async Task OnUpdateModel(IModifyContext ctx)
		{
			var e = ctx.Editable;
			var m = ctx.Model;

			UIEnsure.HasContent(e.Name.Trim(),"请输入姓名");
			UIEnsure.HasContent(e.Ident.Trim(), "请输入账号");

			m.Ident = e.Ident.Trim();
			var time = Now;
			m.Update(e, time);

			var items = await MenuItemSet.Value.LoadListAsync(i => i.MenuId == m.Id);
			var newIdents =await IdentGenerator.BatchGenerateAsync(
				"系统菜单项",
				ADT.Tree.AsEnumerable(e.Items,ii=>ii.Children).Count(i => i.Id == 0)
				);

			MenuItemSet.Value.MergeTree(
				items,
				e.Items,
				mi => mi.Id,
				ei => ei.Id,
				ei => ei.ParentId ?? 0,
				ei => ei.Children,
				(ei, pmi, chds) => {
					var mi = new TMenuItem();
					mi.Id = newIdents.Dequeue();
					mi.Create(time);
					mi.Update(ei, time);
					mi.Action = ei.Action;
					mi.ServiceId = ei.ServiceId;
					mi.ActionArgument = ei.ActionArgument;
					mi.ParentId = pmi?.Id;
					mi.MenuId = m.Id;
					mi.Children = chds;
					return mi;
				},
				(mi, ei) =>
				{
					mi.Update(ei, time);
					mi.ParentId = ei.ParentId;
					mi.Action = ei.Action;
					mi.ActionArgument = ei.ActionArgument;
					mi.ServiceId = ei.ServiceId;
				}
				);

		}
		
		public async Task<MenuItem[]> GetMenu(string Ident)
		{
			var scopeid = ServiceInstanceDescriptor.ParentInstanceId;
			var menuId = await DataSet
				.AsQueryable()
				.Where(m=>m.ScopeId== scopeid && m.Ident==Ident)
				.Select(i => i.Id)
				.SingleOrDefaultAsync();

			if (menuId == 0)
				return null;

			var items = await MenuItemSet.Value.AsQueryable()
				.Where(i => i.MenuId == menuId)
				.SelectUIObjectEntity(i =>
				  new MenuItem
				  {
					  Id = i.Id,
					  ParentId=i.ParentId,
					  Action = i.Action,
					  ServiceId=i.ServiceId,
					  ActionArgument = i.ActionArgument
				  }).ToArrayAsync();

			return ADT.Tree.Build(
				items,
				i => i.Id,
				i => i.ParentId ?? 0,
				(p, i) =>
				{
					if (p.Children == null)
						p.Children = new List<MenuItem>();
					((List<MenuItem>)p.Children).Add(i);
				}
				).ToArray();
		}
	}

}
