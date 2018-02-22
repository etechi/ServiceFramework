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
		public ConsoleMenuItem[] GetUserMenuItems(ClaimsPrincipal User)
		{
			return MenuItems;
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

	public class ConsoleService : IBackEndAdminConsoleService
	{
		IAccessToken AccessToken { get; }
		IEntityCache<string,CachedConsole> EntityCache { get; }
		public ConsoleService(
			IAccessToken AccessToken,
			IEntityCache<string, CachedConsole> EntityCache
			)
		{
			this.EntityCache = EntityCache;
			this.AccessToken = AccessToken;
		}
		Task<CachedConsole> GetCachedConsole(string ConsoleIdent)
		{
			return EntityCache.Find(ConsoleIdent);
		}
		public async Task<Console> GetConsole(string ConsoleIdent)
		{
			var c = await GetCachedConsole(ConsoleIdent);

			return new Console
			{
				Title = c.Title,
				SystemVersion = c.SystemVersion,
				MenuItems = c.GetUserMenuItems(AccessToken.User)
			};
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

