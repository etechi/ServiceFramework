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

using SF.Sys.Annotations;
using SF.Sys.Auth;
using SF.Sys.Clients;
using SF.Sys.Entities;
using SF.Sys.NetworkService;
using SF.Sys.Reflection;
using SF.Sys.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SF.Sys.BackEndConsole.Front
{
	
	public class CachedConsole: Console
	{
		public Dictionary<string,Page> Pages { get; set; }
		
		
		public ConsoleMenuItem[] GetUserMenuItems(
			ClaimsPrincipal User,
			IAuthService AuthService,
			IEntityMetadataCollection EntityMetadataCollection
			)
		{
			bool PermissionCheck(string Permission)
			{
				if (Permission.IsNullOrEmpty())
					return true;

				//检查实体浏览权限
				if (Permission.StartsWith("@"))
				{
					var em = EntityMetadataCollection.FindByTypeIdent(Permission.Substring(1));
					if (em == null)
						return false;
					if (em.EntityManagerType == null)
						return false;
					var re = EntityPermissionType.None;
					if (!em.EntityManagerCapability.HasFlag(EntityCapability.Queryable))
						return false;
					return AuthService.Authorize(
						User, 
						em.EntityManagerType.GetFullName(), 
						nameof(Managers.IConsoleManager.QueryAsync), 
						null
						);

				}
				//普通权限
				var pair = Permission.LastSplit2('.');
				return AuthService.Authorize(
					User,
					pair.Item1,
					pair.Item2,
					null
					);
			}
			
			IEnumerable<ConsoleMenuItem> filter(IEnumerable<ConsoleMenuItem> items)
			{
				foreach(var i in items)
				{
					var re = PermissionCheck(i.Permission);
					if (!re)
						continue;
					var nc = i.Children == null ? null : filter(i.Children).ToList();
					if ((nc?.Count ?? 0) == 0 && i.Link == null)
						continue;
					yield return new ConsoleMenuItem
					{
						Children = nc,
						FontIcon = i.FontIcon,
						Link = i.Link,
						Permission = i.Permission,
						Title = i.Title
					};
				}

			}
			return filter(MenuItems).ToArray();
		}
		public Task<Page> GetUserPage(string PagePath,ClaimsPrincipal User)
		{
			if (!Pages.TryGetValue(PagePath, out var p))
				p = null;
			return Task.FromResult(p);
		}
	}
	public static class MenuItemExtensions
	{
	
	}
	/// <summary>
	/// 管理控制台服务
	/// </summary>
	public class ConsoleService : IBackEndAdminConsoleService
	{
		IAccessToken AccessToken { get; }
		IEntityCache<string,CachedConsole> EntityCache { get; }
		IEntityMetadataCollection EntityMetadataCollection { get; }
		IAuthService AuthService { get; }
		public ConsoleService(
			IAccessToken AccessToken,
			IEntityCache<string, CachedConsole> EntityCache,
			IAuthService AuthService,
			IEntityMetadataCollection EntityMetadataCollection
			)
		{
			this.AuthService = AuthService;
			this.EntityCache = EntityCache;
			this.AccessToken = AccessToken;
			this.EntityMetadataCollection = EntityMetadataCollection;
		}
		Task<CachedConsole> GetCachedConsole(string ConsoleIdent)
		{
			return EntityCache.Find(ConsoleIdent);
		}
		EntityPermissionType GetEntityPermission(string Entity)
		{
			var User = AccessToken.User;
			var em = EntityMetadataCollection.FindByTypeIdent(Entity);
			if (em == null)
				return EntityPermissionType.None;
			if (em.EntityManagerType == null)
				return EntityPermissionType.None;
			var re = EntityPermissionType.None;
			var mname = em.EntityManagerType.GetFullName();

			bool check(EntityCapability flag, string method) =>
				em.EntityManagerCapability.HasFlag(EntityCapability.Queryable) &&
				AuthService.Authorize(User, mname, method, null);

			if (!check(EntityCapability.Queryable, nameof(Managers.IConsoleManager.QueryAsync)))
				return re;
			re = EntityPermissionType.ReadOnly;

			if (!check(EntityCapability.Updatable, nameof(Managers.IConsoleManager.UpdateAsync)))
				return re;
			re = EntityPermissionType.Edit;

			if (!check(EntityCapability.Creatable, nameof(Managers.IConsoleManager.CreateAsync)))
				return re;
			re = EntityPermissionType.NewAndEdit;

			if (!check(EntityCapability.Removable, nameof(Managers.IConsoleManager.RemoveAsync)))
				return re;
			re = EntityPermissionType.Full;
			return re;
		}
		public async Task<Console> GetConsole(string ConsoleIdent)
		{
			var c = await GetCachedConsole(ConsoleIdent);

			return new Console
			{
				Title = c.Title,
				SystemVersion = c.SystemVersion,
				MenuItems = c.GetUserMenuItems(AccessToken.User, AuthService, EntityMetadataCollection),
				EntityPermissions= EntityMetadataCollection
					.Select(e=>(id:e.Ident,p:GetEntityPermission(e.Ident)))
					.Where(e=>e.p!=EntityPermissionType.None)
					.ToDictionary(e=>e.id,e=>e.p)
			};
		}

		public Task<bool[]> CheckPermissions(string[] Methods)
		{
			var re = Methods.Select(m => m.Split2('.')).Select(
				m => AuthService.Authorize(AccessToken.User, m.Item1, m.Item2, null)
				).ToArray();
			return Task.FromResult(re);
		}

		public async Task<Page> GetPage(string ConsoleIdent, string PagePath)
		{
			var c = await GetCachedConsole(ConsoleIdent);
			return await c.GetUserPage(PagePath, AccessToken.User);
		}

		public Task<ConsoleMenuItem[]> HotMenuList()
		{
			throw new System.NotImplementedException();
		}

		public Task HotMenuUpdate(ConsoleMenuItem[] Items)
		{
			throw new System.NotImplementedException();
		}

		public Task<long> HotQueryCreateOrUpdate(HotQuery Query)
		{
			throw new System.NotImplementedException();
		}

		public Task HotQueryRemove(long Id)
		{
			throw new System.NotImplementedException();
		}

		public Task<HotQuery[]> HotQuerySearch(HotQueryListArgument Arg)
		{
			throw new System.NotImplementedException();
		}
	}

}

