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
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SF.Sys.Reminders.Models
{
	[EntityObject]
	public class RemindRecord : EventEntityBase
	{
		[MaxLength(100)]
		[Required]
		public string Name { get; set; }

		[MaxLength(100)]
		[Required]
		public string Action { get; set; }

		[MaxLength(200)]
		public string Description { get; set; }

		[MaxLength(200)]
		public string Error { get; set; }

		[EntityIdent(typeof(Reminder),nameof(ReminderName))]
		public long ReminderId { get; set; }


		public string ReminderName { get; set; }

		[MaxLength(100)]
		[Required]
		public string BizIdent { get; set; }
	}
}
