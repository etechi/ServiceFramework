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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.Sys.Entities;
namespace SF.Sys.Plans.Manager
{
	public enum ActionResultType
	{
		None,
		Return,
		ExecPlan,
		DelayTo,
		WaitEvent,
		WaitUserInput
	}
	public interface IActionResult
	{
		ActionResultType Type { get; }
	}
	public class NoneResult : IActionResult
	{
		public ActionResultType Type => ActionResultType.None;
	}
	public class ReturnResult : IActionResult
	{
		public ActionResultType Type => ActionResultType.Return;
		public object Result { get; set; }
	}
	public class ExecPlanResult : IActionResult
	{
		public ActionResultType Type => ActionResultType.ExecPlan;
		public long PlanId { get; set; }
	}
	public class DelayToResult : IActionResult
	{
		public ActionResultType Type => ActionResultType.DelayTo;
		public DateTime Target { get; set; }
	}
	public class WaitEventResult : ActionResult
	{
		public override ActionResultType Type => ActionResultType.WaitEvent;
		public Type EventType { get; set; }
		public DateTime Expires { get; set; }
	}
	public class WaitUserInputResult : ActionResult
	{
		public override ActionResultType Type => ActionResultType.WaitUserInput;
		public Type UserInputType { get; set; }
		public DateTime Expires { get; set; }
	}
	public interface IActionExecContext
	{
		bool IsCallback { get; set; }
		string CallbackContext { get; set; }
		string Options { get; }
		string Argument { get; }
		object Data { get; }
	}
	public interface IActionProvider
	{
		Task<IActionResult> Execute(IActionExecContext Context);
	}
}
