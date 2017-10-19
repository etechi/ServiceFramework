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

using SF.Metadata;
using SF.Auth;
using SF.Auth.Identities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.Auth.Identities.Models;
using SF.Entities;
using SF.Data;
using SF.Core.ServiceManagement;
using SF.Core;
using SF.Management.MenuServices;
using SF.Core.ServiceManagement.Management;
using System.Reflection;

namespace SF.Core.ServiceManagement
{
	
	public static class MenuServiceExtension
	{
		static async Task CollectMenuItem(
			   Models.ServiceInstanceInternal svc,
			   IServiceInstanceManager sim,
			   IServiceDeclarationTypeResolver svcTypeResolver,
			   IEntityMetadataCollection EntityMetadataCollection,
			   List<MenuItem> items
			   )
		{
			var type = svcTypeResolver.Resolve(svc.ServiceType);
			var entity = EntityMetadataCollection.FindByManagerType(type);
			//var em = type.GetCustomAttribute<EntityManagerAttribute>();
			if (entity != null)
				items.Add(new MenuItem
				{
					Name = type.Comment().Name,//svcTypeResolver.GetTypeIdent(type),
					Title = type.Comment().Name,
					Action = MenuActionType.EntityManager,
					ActionArgument = entity.Ident,// svcTypeResolver.GetTypeIdent(type),
					ServiceId = svc.Id
				});
			var re = await sim.QueryAsync(
				new ServiceInstanceQueryArgument
				{
					ContainerId = svc.Id
				}, Paging.Default
				);
			foreach (var i in re.Items)
				await CollectMenuItem(i, sim, svcTypeResolver, EntityMetadataCollection, items);
		}
		public static async Task<MenuItem[]> GetServiceMenuItems(this IServiceInstanceManager sim, IServiceProvider sp, long ServiceId)
		{
			var svcTypeResolver = sp.Resolve<IServiceDeclarationTypeResolver>();
			var EntityMetadataCollection = sp.Resolve<IEntityMetadataCollection>();
			var svc = await sim.GetAsync(ObjectKey.From(ServiceId));
			var items = new List<MenuItem>();
			await CollectMenuItem(svc, sim, svcTypeResolver, EntityMetadataCollection, items);
			return items.ToArray();
		}
		public static IServiceCollection AddDefaultMenuItems(
			this IServiceCollection sc,
			string MenuIdent,
			string Path,
			params MenuItem[] MenuItems
			)
		{
			if (MenuItems.Length == 0)
				return sc;
			sc.AddInitializer("service", "设置默认菜单项:"+Path, (isp) =>
			  {
				  var dmc = isp.Resolve<IDefaultMenuCollection>();
				  dmc.AddMenuItems(
					  MenuIdent,
					  Path,
					  MenuItems
					  );
				  return Task.CompletedTask;
			  });
			return sc;

		}
		public static IServiceInstanceInitializer<T> WithSystemAdminMenuItems<T>(
			this IServiceInstanceInitializer<T> sii,
			string Path,
			params MenuItem[] MenuItems
			)
			=> sii.WithMenuItems("system", Path, MenuItems);
		public static IServiceInstanceInitializer<T> WithBiznessAdminMenuItems<T>(
			this IServiceInstanceInitializer<T> sii,
			string Path,
			params MenuItem[] MenuItems
			)
			=> sii.WithMenuItems("bizness", Path, MenuItems);

		public static IServiceInstanceInitializer<T> WithMenuItems<T>(
			this IServiceInstanceInitializer<T> sii,
			string MenuIdent,
			string Path,
			params MenuItem[] MenuItems
			)
		{	
			sii.AddAction(async (sp, sid) =>
			{
				var dmc=sp.Resolve<IDefaultMenuCollection>();
				var sim = sp.Resolve<IServiceInstanceManager>();
				dmc.AddMenuItems(
					MenuIdent,
					Path,
					await sim.GetServiceMenuItems(sp, sid)
					);
				if (MenuItems.Length > 0)
				{
					foreach (var mi in MenuItems)
						mi.ServiceId = sid;

					dmc.AddMenuItems(
						MenuIdent,
						Path,
						MenuItems
						);
				}
			});
			return sii;
		}
	}

}

