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
using SF.Sys.Entities;
using SF.Sys.Entities.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SF.Sys.Reminders.Models
{

	/// <summary>
	/// 提醒
	/// </summary>
	[EntityObject]
	public class Reminder : SF.Sys.AtLeastOnceTasks.Models.AtLeastOnceTaskEntityBase<long>
	{      
		/// <summary>
		/// 业务类型
		/// </summary>
		[MaxLength(100)]
		[Required]
		public virtual string BizType { get; set; }


		/// <summary>
		/// 业务标识类型
		/// </summary>
		[EntityType]
		public string BizIdentType { get; set; }

		/// <summary>
		/// 业务标识
		/// </summary>
		[EntityIdent(null,nameof(BizIdentName),EntityTypeField =nameof(BizIdentType))]
		public long BizIdentIdent { get; set; }

		/// <summary>
		/// 业务标识
		/// </summary>
		[TableVisible]
		[Ignore]
		public string BizIdentName { get; set; }

		/// <summary>
		/// 提醒数据
		/// </summary>
		[MaxLength(1000)]
		public string Data { get; set; }

		/// <summary>
		/// 提醒名称
		/// </summary>
		[MaxLength(100)]
		[Required]
		public string RemindableName { get; set; }
	}
}
