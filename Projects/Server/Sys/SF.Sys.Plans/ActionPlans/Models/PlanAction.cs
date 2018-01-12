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
using SF.Sys.Entities.DataModels;
using SF.Sys.Entities.Models;
using SF.Sys.Services.Management.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Sys.ActionPlans.Models
{


	[EntityObject]
	public class PlanAction : TreeNodeEntityBase<PlanAction>
	{
		[EntityIdent(typeof(ActionPlan),nameof(PlanName))]
		public long PlanId{get;set;}

		public string PlanName { get; set; }

		/// <summary>
		/// 动作类型
		/// </summary>
		[EntityIdent(typeof(ServiceInstance),nameof(ActionProviderName))]
		public long ActionProviderId { get; set; }

		/// <summary>
		/// 动作名称
		/// </summary>
		public string ActionProviderName { get; set; }
		
		/// <summary>
		/// 动作设置
		/// </summary>
		public string Options { get; set; }

		/// <summary>
		/// 超时(s)
		/// </summary>
		public int ExpireSeconds { get; set; }

		/// <summary>
		/// 错误延时
		/// </summary>
		public int DelaySecondsOnError { get; set; }
	}
}
