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
		ModidifiableEntityManager<ObjectKey<long>, Models.Menu,  MenuQueryArgument, Models.MenuEditable, TMenu>,
		IMenuService
		where TMenu: DataModels.Menu<TMenu,TMenuItem>,new()
		where TMenuItem : DataModels.MenuItem<TMenu, TMenuItem>, new()
	{
		public Lazy<IDataSet<TMenuItem>> MenuItemSet { get; }
		
		public EntityMenuService(
			IDataSetEntityManager<Models.MenuEditable, TMenu> Manager,
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
			m.Id = await IdentGenerator.GenerateAsync();
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
			foreach (var n in ADT.Tree.AsEnumerable(e.Items, ii => ii.Children).Where(i => i.Id == 0))
				n.Id = await IdentGenerator.GenerateAsync();

			MenuItemSet.Value.MergeTree(
				null,
				items,
				e.Items,
				mi => mi.Id,
				ei => ei.Id,
				ei => ei.ParentId ?? 0,
				ei => ei.Children,
				(ei, pmi) => {
					var mi = new TMenuItem();
					mi.Id = ei.Id;
					mi.Create(time);
					mi.Update(ei, time);
					mi.Action = ei.Action;
					mi.ServiceId = ei.ServiceId;
					mi.ActionArgument = ei.ActionArgument;
					mi.MenuId = m.Id;
					mi.ParentId = pmi?.Id;
					return mi;
				},
				(mi, ei,np) =>
				{
					mi.Update(ei, time);
					mi.ParentId = np?.Id;
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
