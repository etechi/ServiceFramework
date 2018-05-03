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


using SF.Sys.Entities.Models;
using SF.Sys.Annotations;
using System.Collections.Generic;
using System;

namespace SF.Common.Tickets
{
	/// <summary>
	/// 工单
	/// </summary>
	[EntityObject]
	public class Ticket : ObjectEntityBase
	{
	

		/// <summary>
		/// 工单内容,Html格式
		/// </summary>
		public string Content { get; set; }

		/// <summary>
		/// 工单图片
		/// </summary>
		public TicketImage[] Images { get; set; }

		/// <summary>
		/// 状态
		/// </summary>
		public TicketState State { get; set; }


		/// <summary>
		/// 回复
		/// </summary>
		public TicketReply[] Replies { get; set; }

	}
	public class TicketReply : ObjectEntityBase
	{
		/// <summary>
		/// 是否为客服回复
		/// </summary>
		public bool IsAdmin { get; set; }

		/// <summary>
		/// 回复人
		/// </summary>
		public long OwnerId { get; set; }

		/// <summary>
		/// 回复人
		/// </summary>
		public string OwnerName { get; set; }

		/// <summary>
		/// 回复内容,Html格式
		/// </summary>
		public string Content { get; set; }

		/// <summary>
		/// 回复图片
		/// </summary>
		public TicketImage[] Images { get; set; }
	}
	
}
