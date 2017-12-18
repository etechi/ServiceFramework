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

using SF.Sys.Entities.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Sys.Plans.Manager.Models
{
	public enum ActionType
	{
		Call,
		ExecutePlan,
		Delay,
		DelayTo
	}
	
	public class Action 
	{
		public string Name;

	}
	public class CallAction : Action
	{
		public string Type { get; set; }
		public string Argument { get; set; }
		public int ExpireSeconds { get; set; }
		public int DelaySecondsOnError { get; set; }
	}
	public class ExecutePlanAction : Action
	{
		public long PlanId { get; set; }
		public string Argument { get; set; }
	}
	public class DelayToAction: Action
	{
		public DateTime Time { get; set; }
	}
	public class DelayAction : Action
	{
		public TimeSpan Time { get; set; }
	}

	public class ActionPlan : ObjectEntityBase
	{
		
		public bool Repeatable { get; set; }
		public Action[] Actions { get; set; }
	}

	public class ActionPlanExecutor
	{
		[Key]
		[Required]
		[MaxLength(100)]
		[Column(Order = 1)]
		public string Type { get; set; }

		[Key]
		[Required]
		[MaxLength(100)]
		[Column(Order = 1)]
		public string Ident { get; set; }

		[Required]
		[MaxLength(100)]
		public string Name { get; set; }


		public long PlanId { get; set; }
	}
}
