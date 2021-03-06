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
using SF.Sys.BackEndConsole.Entity.DataModels;
using SF.Sys.BackEndConsole.Managers;
using SF.Sys.BackEndConsole.Front;
using System.Collections.Generic;
using SF.Sys.Data;
using SF.Sys.Events;
using SF.Utils.TableExports.Excel;
using SF.Utils.TableExports;

namespace SF.Sys.Services
{
	public static class BackEndConsoleServicesDIExtension
	{

		public static IServiceCollection AddBackEndConsoleServices(
			this IServiceCollection sc,
			string Title,
			string TablePrefix = null
			)
		{

			sc.AddDataModules<
				DataConsole,
				DataHotMenuCategory,
				DataHotMenuItem,
				DataUISetting
				>(
				TablePrefix ?? "BackEndAdmin"
				);
			sc.AddScoped<IBackEndConsoleExportService, BackEndConsoleExportService>();
			sc.AddSingleton<IBackEndConsoleBuilderCollection, BackEndConsoleBuilderCollection>();
			sc.AddManagedScoped<IBackEndAdminConsoleService, ConsoleService>();

			sc.AddTransient(sp=>(IBackEndConsoleUISettingService)sp.Resolve<IUISettingManager>());

			sc.EntityServices(
				"BackEndAdmin",
				"后台管理",
				d =>
					d.Add<IConsoleManager, ConsoleManager>("BackEndAdminConsole", "管理控制台")
					.Add<IUISettingManager, UISettingManager>("BackEndAdminUISetting", "控制台设置")
					.Add<IHotMenuCategoryManager, HotMenuCategoryManager>("BackEndAdminHotMenuCategory", "常用菜单分类")
					.Add<IHotMenuItemManager, HotMenuItemManager>("BackEndAdminHotMenuItem", "常用菜单项")
				);

			sc.InitServices("后台管理", async (sp, sim, scope) =>
			{
				var MenuPath = "系统管理/后台管理";
				await sim.Service<IConsoleManager, ConsoleManager>(null)
					.WithConsolePages(MenuPath)
					.Ensure(sp, scope);

				await sim.Service<IUISettingManager, UISettingManager>(null)
					.WithConsolePages(MenuPath)
					.Ensure(sp, scope);

				await sim.Service<IHotMenuCategoryManager, HotMenuCategoryManager>(null)
					.WithConsolePages(MenuPath)
					.Ensure(sp, scope);

				await sim.Service<IHotMenuItemManager, HotMenuItemManager>(null)
					.WithConsolePages(MenuPath)
					.Ensure(sp, scope);

				await sim.Service<IBackEndAdminConsoleService, ConsoleService>(new{})
				.Ensure(sp, scope);

			});
			sc.AddInitializer(
				"service",
				"初始化默认控制台",
				sp => InitDefaultConsole(sp,Title),
				int.MaxValue
				);

			sc.AddEntityGlobalCache(
				async (IDataScope scope, string Ident) =>
				{
					var re = await scope.Use("查询控制台", ctx =>
						  ctx.Queryable<DataConsole>()
						  .Where(c => c.Ident == Ident)
						  .Select(c => new { c.Id, c.Name, c.Pages, c.Menus })
						  .SingleOrDefaultAsync()
						);
					return new CachedConsole
					{
						Id = re.Id,
						Ident = Ident,
						MenuItems = Json.Parse<ConsoleMenuItem[]>(re.Menus),
						Title = re.Name,
						Pages = Json.Parse<Page[]>(re.Pages).ToDictionary(p=>p.Path),
						SystemVersion = "1.0"
					};
				},
				(IServiceProvider isp,
				IEventSubscriber<EntityChanged<ObjectKey<long>, ConsoleEditable>> OnConsoleModified, 
				IEntityCacheRemover<string> remover
				) =>
				{
					var dss = isp.Resolve<IScoped<IDataScope>>();
					OnConsoleModified.Wait(async e =>
					{
						var re = await dss.Use(ds => 
							ds.Use("查询控制台ID", ctx =>
								ctx.Queryable<DataConsole>()
									.Where(c => c.Id == e.Payload.EntityId.Id)
									.Select(c => c.Ident)
									.SingleOrDefaultAsync()
							)
						);
						await remover.Remove(re);
					});
				}
			);

			sc.AddSingleton<ITableExporterFactory, ExcelExporterFactory>("excel");
			return sc;
		}

		static async Task InitDefaultConsole(IServiceProvider sp,string Title)
		{
			var cm = sp.Resolve<IConsoleManager>();

			var ctxDict = new Dictionary<string, BackEndConsoleBuildContext>();
			var coll = (BackEndConsoleBuilderCollection)
				sp.Resolve<IBackEndConsoleBuilderCollection>();
			var usm = sp.Resolve<IUISettingManager>();

			foreach (var (i, b) in coll.Builders)
			{
				if (!ctxDict.TryGetValue(i, out var ctx))
					ctxDict[i] = ctx = new BackEndConsoleBuildContext(Title, i, sp);
				await b(sp, ctx);
			}
			foreach (var ctx in ctxDict.Values)
			{
				var c = ctx.Create();
				var console=await cm.EnsureEntity(
					await cm.QuerySingleEntityIdent(new ConsoleQueryArgument { Ident = c.Ident }),
					e =>
					{
						e.Ident = c.Ident;
						e.Name = c.Title;
						e.Pages = c.Pages.Values.ToArray();
						e.Menus = c.MenuItems;
					});
				foreach (var s in ctx.Settings)
					await usm.EnsureEntity(
						await usm.QuerySingleEntityIdent(new UISettingQueryArgument
						{
							ConsoleId = console.Id,
							Name = s.Key.Name,
							OwnerId = 0,
							Path = s.Key.Path,
						}), 
						e =>
						{
							e.ConsoleId = console.Id;
							e.Name = s.Key.Name;
							e.Path = s.Key.Path;
							e.Value = s.Value;
						}
						);
			}
		}
	}
}