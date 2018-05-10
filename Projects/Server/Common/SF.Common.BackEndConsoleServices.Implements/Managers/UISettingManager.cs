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


using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using SF.Sys.Entities;
using SF.Sys.Data;
using SF.Sys.BackEndConsole.Models;
using SF.Sys.Linq;
using System.Collections;
using SF.Sys.BackEndConsole.Entity.DataModels;
using SF.Sys.Auth;

namespace SF.Sys.BackEndConsole.Managers
{
	public class UISettingManager :
		AutoModifiableEntityManager<ObjectKey<long>,UISetting,UISetting,UISettingQueryArgument,UISetting,DataUISetting>,
		IUISettingManager,
		Front.IBackEndConsoleUISettingService
	{
		public UISettingManager(
			IEntityServiceContext ServiceContext
			) : base(ServiceContext)
		{

		}

		public async Task<Front.UISetting[]> List(long ConsoleId,string Path)
		{
			var uid=ServiceContext.AccessToken.User.EnsureUserIdent();

			var re=await DataScope.Use("获取查询", ctx =>
				 (from q in ctx.Queryable<DataUISetting>()
				  where (!q.OwnerId.HasValue || q.OwnerId.Value == uid) && 
						q.ConsoleId==ConsoleId &&
						q.Path == Path && 
						q.LogicState == EntityLogicState.Enabled
				  orderby q.OwnerId.HasValue?0:1
				  select new Front.UISetting
				  {
					  ConsoleId=q.ConsoleId,
					  Name = q.Name,
					  Path = q.Path,
					  Value = q.Value
				  }).ToArrayAsync()
			);

			return re
				.GroupBy(q => q.Name)
				.Select(g => g.First())
				.Where(q=>q.Value.HasContent())
				.OrderBy(q=>q.Name)
				.ToArray();

		}

		public async Task Update(Front.UISetting Query)
		{
			var uid = ServiceContext.AccessToken.User.EnsureUserIdent();
			var id = await this.QuerySingleEntityIdent(new UISettingQueryArgument
				{
					ConsoleId = Query.ConsoleId,
					Name = Query.Name,
					OwnerId = uid,
					Path = Query.Path,
				});

			if (Query.Value.HasContent())
			{
				if (id == null)
					await CreateAsync(new UISetting
					{
						Name = Query.Name,
						Path = Query.Path,
						Value = Query.Value,
						OwnerId = uid,
						ConsoleId = Query.ConsoleId,
					});
				else
				{
					var e = await LoadForEdit(id);
					e.Value = Query.Value;
					await UpdateAsync(e);
				}
			}
			else
			{
				var gid = await DataScope.Use("查找全局查询", ctx =>
					   (from q in ctx.Queryable<DataUISetting>()
						where q.ConsoleId == Query.ConsoleId &&
						 q.Name == Query.Name &&
						 !q.OwnerId.HasValue &&
						 q.Path == Query.Path 
						select q.Id
					   ).SingleOrDefaultAsync()
					);
				if (gid == 0)
				{
					if (id != null)
						await RemoveAsync(id);
				}
				else if (id == null)
				{
					await CreateAsync(new UISetting
					{
						Name = Query.Name,
						Path = Query.Path,
						Value = null,
						OwnerId = uid,
						ConsoleId = Query.ConsoleId
					});
				}
				else
				{ 
					var e = await LoadForEdit(id);
					e.Value = null;
					await UpdateAsync(e);
				}
			}
		}
	}

}
