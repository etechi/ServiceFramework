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

using SF.Sys.Entities;
using SF.Sys.Services;
using SF.Sys.Services.Management;
using SF.Sys.Services.Management.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SF.Sys.BackEndConsole.Front
{
	class BackEndConsoleBuildContext : IBackEndConsoleBuildContext
	{
		public string Title { get; }
		public string Ident { get; }
		public IServiceProvider ServiceProvider { get; }
		List<ConsoleMenuItem> MenuItems { get;  } = new List<ConsoleMenuItem>();
		Dictionary<string, Page> Pages { get; } = new Dictionary<string, Page>();

		public BackEndConsoleBuildContext(string Title ,string Ident, IServiceProvider ServiceProvider)
		{
			this.Title = Title;
			this.Ident = Ident;
			this.ServiceProvider = ServiceProvider;
		}
		public void AddMenuItems(params MenuItemConfig[] Items)
		{
			foreach(var item in Items)
			{
				var items = MenuItems;
				ConsoleMenuItem last = null;
				foreach(var name in item.Path.Split('/'))
				{
					var idx = items.FindIndex(i => i.Title == name);
					if (idx == -1)
						items.Add(last = new ConsoleMenuItem
						{
							Title = name
						});
					else
						last = items[idx];
					items = last.Children;
					if (items == null)
						last.Children = items=new List<ConsoleMenuItem>();
				}
				last.FontIcon = item.FontIcon;
				last.Link = item.Link;
				last.Permission = item.Permission;
			}
		}

		public void AddPage(Page Page)
		{
			if (Pages.ContainsKey(Page.Path))
				throw new InvalidOperationException("后台控制台页面已存在:" + Page.Path);
			Pages[Page.Path] = Page;
		}

		public CachedConsole Create()
		{
			return new CachedConsole
			{
				MenuItems = MenuItems.ToArray(),
				Pages = Pages,
				Title = Title,
				Ident = Ident,
				SystemVersion = "1.0"
			};
		}

		void BuildEntityPages(
			string MenuPath,
			IEntityMetadata Entity,
			long? ServiceId
			)
		{
			if (!ServiceId.HasValue)
				ServiceId = 0;

			var EntityIdent = Entity.Ident;
			AddMenuItems(new MenuItemConfig
			{
				Path = MenuPath + "/" + Entity.Name,
				Link = $"/ap/entity/{EntityIdent}/",
				Permission="@"+EntityIdent
			});
			//AddPage(new Page
			//{
			//	Path = $"svc/{ServiceId}/entity/{EntityIdent}/list",
			//	Title = Entity.Name,
			//	Content = new PageContent
			//	{
			//		Config = Json.Stringify(new { entity = EntityIdent, service = ServiceId }),
			//		Type = "EntityList",
			//		ResAccessRequest = EntityIdent + ":" + Auth.Permissions.Operations.Read
			//	}
			//});

			//AddPage(new Page
			//{
			//	Path = $"svc/{ServiceId}/entity/{EntityIdent}/detail",
			//	Title = "查看" + Entity.Name,
			//	Content = new PageContent
			//	{
			//		Config = Json.Stringify(new { entity = EntityIdent, service = ServiceId }),
			//		Type = "EntityDetail",
			//		ResAccessRequest = EntityIdent + ":" + Auth.Permissions.Operations.Read
			//	}
			//});

			//if (Entity.EntityManagerCapability.HasFlag(EntityCapability.Creatable))
			//	AddPage(new Page
			//	{
			//		Path = $"svc/{ServiceId}/entity/{EntityIdent}/new",
			//		Title = "添加" + Entity.Name,
			//		Content = new PageContent
			//		{
			//			Config = Json.Stringify(new { entity = EntityIdent, service = ServiceId }),
			//			Type = "EntityNew",
			//			ResAccessRequest= EntityIdent + ":"+Auth.Permissions.Operations.Create
			//		}
			//	});

			//if (Entity.EntityManagerCapability.HasFlag(EntityCapability.Updatable))
			//	AddPage(new Page
			//	{
			//		Path = $"svc/{ServiceId}/entity/{EntityIdent}/edit",
			//		Title = "编辑" + Entity.Name,
			//		Content = new PageContent
			//		{
			//			Config = Json.Stringify(new { entity = EntityIdent, service = ServiceId }),
			//			Type = "EntityEdit",
			//			ResAccessRequest = EntityIdent + ":" + Auth.Permissions.Operations.Update
			//		}
			//	});
		}
		async Task BuildConsolePages(
			string MenuPath,
			ServiceInstanceInternal svc,
			IServiceInstanceManager sim,
			IServiceDeclarationTypeResolver svcTypeResolver,
			IEntityMetadataCollection EntityMetadataCollection
			)
		{
			var type = svcTypeResolver.Resolve(svc.ServiceType);
			var entity = EntityMetadataCollection.FindByManagerType(type);
			if (entity != null)
				BuildEntityPages(
					MenuPath,
					entity,
					svc.Id
					);


			//构造子服务页面
			var re = await sim.QueryAsync(
				new ServiceInstanceQueryArgument
				{
					ContainerId = svc.Id,
					Paging = Paging.Default
				}
				);
			foreach (var i in re.Items)
				await BuildConsolePages(
					MenuPath,
					i,
					sim,
					svcTypeResolver,
					EntityMetadataCollection
					);
		}
		
		public async Task AddEntityManager(string MenuPath, long EntityServiceId)
		{
			var svcTypeResolver = ServiceProvider.Resolve<IServiceDeclarationTypeResolver>();
			var EntityMetadataCollection = ServiceProvider.Resolve<IEntityMetadataCollection>();
			var sim = ServiceProvider.Resolve<IServiceInstanceManager>();
			var svc = await sim.GetAsync(ObjectKey.From(EntityServiceId));
			await BuildConsolePages(
				MenuPath,
				svc,
				sim,
				svcTypeResolver,
				EntityMetadataCollection
				);
		}
	}
}

