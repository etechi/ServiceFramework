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

using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using SF.Sys.TimeServices;
using System.Linq;
using SF.Sys.CallPlans.Runtime;
using SF.Sys.CallPlans;
using SF.Sys.AtLeastOnceActions.Models;
using SF.Sys.AtLeastOnceActions;
using SF.Sys.Threading;

namespace SF.Sys.Services
{
	class AtLeastOnceActionSyncQueue
	{
		public ISyncQueue<(string,string)> Queue { get; set; }
	}
	public static class AtLeastOnceActionDIExtension
	{
		
		public static IServiceCollection AddAtLeastOnceActionservices(this IServiceCollection sc,string TablePrefix=null)
		{
			sc.AddDataModules<
				   SF.Sys.AtLeastOnceActions.DataModels.AtLeastOnceAction
				   >(
				   TablePrefix ?? "Sys"
				   );

			sc.EntityServices(
				"AtLeastOnceActions",
				"至少一次调用管理",
				d => d
					.Add<IAtLeastOnceActionManager, AtLeastOnceActionManager>("AtLeastOnceAction", "至少一次调用", typeof(AtLeastOnceAction))
				);


			sc.InitServices("至少一次调用管理", async (sp, sim, scope) =>
			{
				var MenuPath = "系统管理/至少一次调用";
				await sim.Service<IAtLeastOnceActionManager, AtLeastOnceActionManager>(null)
					.WithMenuItems(MenuPath)
					.Ensure(sp, scope);

			});


			var syncQueue = new AtLeastOnceActionSyncQueue();
			sc.AddSingleton(sp => syncQueue);
			sc.AddAtLeastOnceEntityTaskService(
				new AtLeastOnceActionEntityServiceSetting<
					(long Id, string Type, string Ident),
					(string Type, string Ident),
					SF.Sys.AtLeastOnceActions.DataModels.AtLeastOnceAction
					>
				{
					Name = "至少一次调用服务",
					GetKey = t => (t.Id, t.Type, t.Ident),
					KeyEqual = t => e => e.Id.Equals(t.Id),
					Init = (sp, sq) =>
					{
						syncQueue.Queue = sq;
						return Task.CompletedTask;
					},
					RunTask = async (sp, entity) =>
					{
						var re=	await ((AtLeastOnceActionProvider)sp.Resolve<IAtLeastOnceActionProvider>()).ActiveByTimer(entity);
						return re;
					}
				});

			return sc;
		}

	}
}