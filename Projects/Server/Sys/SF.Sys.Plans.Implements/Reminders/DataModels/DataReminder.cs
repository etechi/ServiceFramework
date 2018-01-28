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
using SF.Sys.Entities;
using SF.Sys.Entities.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SF.Sys.Reminders.DataModels
{
	[Table("Reminder")]
	public class DataReminder : DataRemindRecord
	{   
		/// <summary>
		/// 业务类型
		/// </summary>
		[Index("biz", IsUnique = true, Order = 1)]
		public override string BizType { get; set; }

		/// <summary>
		/// 业务标识类型
		/// </summary>
		[Index("biz",IsUnique =true,Order =2)]
		public override string BizIdentType { get; set; }

		/// <summary>
		/// 业务标识
		/// </summary>
		[Index("biz",IsUnique =true,Order =3)]
		public override long BizIdent { get; set; }


	}
}
