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

namespace SF.Sys.BackEndConsole.Entity
{
	public class EntityMenuService :
		AutoModifiableEntityManager<ObjectKey<long>,Menu,Menu,MenuQueryArgument,MenuEditable,DataMenu>,
		IMenuService
	{
		public EntityMenuService(
			IEntityServiceContext ServiceContext
			) : base(ServiceContext)
		{

		}

		
		public async Task<MenuItem[]> GetMenu(string Ident)
		{
			var scopeid = ServiceInstanceDescriptor.ParentInstanceId;
			var items = await DataScope.Use("获取菜单项", ctx =>
				 ctx.Set<DataMenu>()
					 .AsQueryable()
					 .Where(m => m.ServiceDataScopeId == scopeid && m.Ident == Ident)
					 .Select(i => i.Items)
					 .SingleOrDefaultAsync()
			 );

			if (items == null)
				return null;
			return Json.Parse<MenuItem[]>(items);
		}
	}

}
