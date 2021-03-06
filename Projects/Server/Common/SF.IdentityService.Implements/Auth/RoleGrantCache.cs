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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using SF.Auth.IdentityServices.Internals;
using SF.Sys;
using SF.Sys.Auth;
using SF.Sys.Data;
using SF.Sys.Entities;
using SF.Sys.Events;
using SF.Sys.Linq;
using SF.Sys.Services;

namespace SF.Auth.IdentityServices
{
	public class RoleGrantCache
	{
		IScoped<IDataScope> DataScope { get; }
		Dictionary<(string,string), HashSet<string>> _GrantRoles;
		public RoleGrantCache(
			IScoped<IDataScope> DataScope,
			IEventSubscriber<EntityChanged<ObjectKey<string>, Models.RoleEditable>> OnRoleModified,
			IEventSubscriber<EntityChanged<ObjectKey<long>, Models.GrantEditable>> OnGrantModified
			)
		{
			OnRoleModified.Wait(e =>
			{
				_GrantRoles = null;
				return Task.CompletedTask;
			});
			OnGrantModified.Wait(e =>
			{
				_GrantRoles = null;
				return Task.CompletedTask;
			});
			this.DataScope = DataScope;
		}
		Dictionary<(string,string), HashSet<string>> LoadGrantRoles()
		{
			var re = Task.Run(
				async () =>
				{
					var items = await DataScope.Use(ds => ds.Use("获取权限角色", ctx =>
						(from r in ctx.Queryable<DataModels.DataRole>()
						 where r.LogicState == EntityLogicState.Enabled
						 from rg in r.Grants
						 let g = rg.DstGrant
						 where g.LogicState == EntityLogicState.Enabled
						 from gi in g.Items
						 select new { gi.ServiceId, gi.ServiceMethodId, rg.RoleId }
						).ToArrayAsync()
						));
					return items;
				}).Result;
			return re
				.GroupBy(r => (r.ServiceId, r.ServiceMethodId), r => r.RoleId)
				.ToDictionary(g => g.Key, g => g.ToHashSet());
		}
		public bool Validate(string Service,string Method,string[] Roles)
		{
			var gr = _GrantRoles;
			if (gr == null)
				_GrantRoles = gr =  LoadGrantRoles();
			if (gr.TryGetValue((Service,Method), out var rs))
				return Roles.Any(r => rs.Contains(r));
			if (gr.TryGetValue((Service, "*"), out rs))
				return Roles.Any(r => rs.Contains(r));
			return false;
		}
	}
}
