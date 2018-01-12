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

using SF.Sys.Data;
using SF.Sys.Events;
using SF.Sys.Entities;
using System.Threading.Tasks;
using SF.Sys.ActionPlans.Runtime;
using SF.Sys.ActionPlans.DataModels;

namespace SF.Sys.Services
{
	public static class ActionPlanDIServiceCollectionExtension
	{
		public static IServiceCollection AddActionPlans(this IServiceCollection sc,string TablePrefix=null)
		{
			sc.AddDataModules<
				   SF.Sys.ActionPlans.DataModels.ActionPlan,
				   SF.Sys.ActionPlans.DataModels.DataActionPlanAction,
				   SF.Sys.ActionPlans.DataModels.ActionPlanExecutor
				   >(
				   TablePrefix ?? "Sys"
				   );
			sc.AddEntityLocalCache(
				async (IDataSet<ActionPlan> set, long Id) =>
				{
					var re=await RuntimePlan.Load(set, Id);
					return re;
				},
				(IEventSubscriber<EntityChanged<ActionPlan>> OnPlanModified, IEntityCacheRemover<long> remover) =>
				{
					OnPlanModified.Wait(e =>
					{
						remover.Remove(e.Id);
						return Task.CompletedTask;
					});
				}
			);
			return sc;
		}
		
	}
}
