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

namespace SF.Common.Notifications.DataModels
{

	/// <summary>
	/// 通知发送策略
	/// </summary>
	[Table("NotificationSendPolicy")]
	public class NotificationSendPolicy : SF.Sys.Entities.DataModels.DataObjectEntityBase
	{
		/// <summary>
		/// 策略标识
		/// </summary>
		[Index(IsClustered =true)]
		[Required]
		[MaxLength(100)]
		public string Ident { get; set; }

		/// <summary>
		/// 策略动作
		/// </summary>
		[JsonData]
		public string Actions { get; set; }

	}

}
