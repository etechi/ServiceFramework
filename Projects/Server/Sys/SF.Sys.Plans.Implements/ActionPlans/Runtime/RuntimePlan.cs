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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SF.Sys.Entities;
using SF.Sys.Linq;

namespace SF.Sys.ActionPlans.Runtime
{
	class RuntimePlan : Dictionary<long,IRuntimeAction>,IRuntimePlan
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public IEnumerable<RuntimeAction> TempActions { get; set; }
		public IRuntimeAction FirstAction { get; set; }
		public IRuntimeAction FindAction(long Id)
		{
			TryGetValue(Id, out var a);
			return a;
		}


		public static async Task<RuntimePlan> Load(IDataScope DataScope, long Id)
		{
			var plan = await DataScope.Use("载入计划", ctx =>
				 ctx.Queryable<DataModels.ActionPlan>()
				 .Where(p => p.Id == Id && p.LogicState == EntityLogicState.Enabled)
				 .Select(p => new RuntimePlan
				 {
					 Id = p.Id,
					 Name = p.Name,
					 TempActions = from a in p.Actions
								   where a.LogicState == EntityLogicState.Enabled
								   orderby a.ItemOrder
								   select new RuntimeAction
								   {
									   Id = a.Id,
									   ParentId = a.ContainerId,
									   Name = a.Name,
									   ActionProviderId = a.ActionProviderId,
									   ActionProviderOptions = a.ActionProviderOptions
								   }
				 })
				 .SingleOrDefaultAsync()
				);
			if (plan == null)
				return plan;

			var nodes = SF.Sys.ADT.Tree.Build(
				plan.TempActions,
				a => a.Id,
				a => a.ParentId ?? 0,
				(p, a) =>
				{
					if (p.LastChildAction != null)
						p.LastChildAction.NextAction = a;
					else
						p.FirstChildAction = a;
					p.LastChildAction = a;
				})
				.Where(n => !n.ParentId.HasValue)
				.Reverse()
				.ToArray();

			if (nodes.Length == 1)
			{
				plan.FirstAction = nodes[0];
				nodes[0].Plan = plan;
			}
			else if (nodes.Length > 1)
			{
				plan.FirstAction = nodes.Reverse().Aggregate((x, y) => { y.NextAction = x; return y; });

				foreach (var a in SF.Sys.ADT.Tree.AsEnumerable(
					ADT.Link.ToEnumerable(plan.FirstAction, a => a.NextAction),
					n => ADT.Link.ToEnumerable(n, a => a.NextAction)
					))
				{
					((RuntimeAction)a).Plan = plan;
					plan.Add(a.Id, a);
				}
			}
			return plan;
		}
	}
}
