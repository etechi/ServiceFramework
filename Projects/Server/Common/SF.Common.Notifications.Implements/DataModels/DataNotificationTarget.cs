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
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SF.Sys.Entities.DataModels;
using SF.Sys.Data;
using SF.Sys.Annotations;
using SF.Common.Notifications.Models;
using SF.Sys.Entities;

namespace SF.Common.Notifications.DataModels
{

	/// <summary>
	/// 通知对象
	/// 普通通知每个通知用户跟随通知对象创建
	/// 全局通知用户读取通知后创建
	/// </summary>
	[Table("NotificationTarget")]
	public class DataNotificationTarget
	{

		[Key]
		[Column(Order = 1)]
		[Index("user", Order = 1)]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		[Display(Name = "用户ID")]
		public long UserId { get; set; }

		[Key]
		[Column(Order = 2)]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		[Display(Name = "通知ID")]
		public long NotificationId { get; set; }

		/// <summary>
		/// 通知模式
		/// </summary>
		public NotificationMode Mode { get; set; }

		[Index("user", Order = 2)]
		[Display(Name = "逻辑状态")]
		public EntityLogicState LogicState { get; set; }

		[Index("user", Order = 3)]
		[Display(Name = "时间")]
		public DateTime Time { get; set; }

		[Display(Name = "已读时间")]
		public DateTime? ReadTime { get; set; }

		[ForeignKey(nameof(NotificationId))]
		public Notification Notification { get; set; }

	}


}
