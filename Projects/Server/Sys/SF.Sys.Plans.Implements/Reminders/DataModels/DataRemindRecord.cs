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
using SF.Sys.AtLeastOnceTasks.DataModels;
using SF.Sys.Data;
using SF.Sys.Entities.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SF.Sys.Reminders.DataModels
{
	[Table("RemindRecord")]
	public class DataRemindRecord : DataAtLeastOnceTaskEntityBase<long>
	{
		/// <summary>
		 /// 业务类型
		 /// </summary>
		[Index("biz", Order = 1)]
		public virtual string BizType { get; set; }

		/// <summary>
		/// 业务标识类型
		/// </summary>
		[Index("biz", Order = 2)]
		public virtual string BizIdentType { get; set; }

		/// <summary>
		/// 业务标识
		/// </summary>
		[Index("biz", Order = 3)]
		public virtual long BizIdent { get; set; }


		[MaxLength(1000)]
		public string Data { get; set; }

		[MaxLength(100)]
		[Required]
		public string RemindableName { get; set; }
	}
}
