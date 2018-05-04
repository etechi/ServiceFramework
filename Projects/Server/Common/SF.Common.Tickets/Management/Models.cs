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
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using SF.Sys.Auth;

namespace SF.Common.Tickets.Management
{
	/// <summary>
	/// 工单分类
	/// </summary>
	[EntityObject]
	public class TicketCategory : ObjectEntityBase
	{
		
	}

	/// <summary>
	/// 工单
	/// </summary>
	[EntityObject]
	public class Ticket : ObjectEntityBase
	{
	
		/// <summary>
		/// 工单分类
		/// </summary>
		[EntityIdent(typeof(TicketCategory), nameof(CategoryName))]
		[Layout(1, 2)]
		public long? CategoryId { get; set; }

		/// <summary>
		/// 分类名称
		/// </summary>
		[TableVisible]
		public string CategoryName { get; set; }

	
		/// <summary>
		/// 发布时间
		/// </summary>
		[TableVisible]
		public DateTime? CreateTime{ get; set; }

		/// <summary>
		/// 状态
		/// </summary>
		[TableVisible]
		public TicketState State { get; set; }

		/// <summary>
		/// 用户
		/// </summary>
		[EntityIdent(typeof(User), nameof(OwnerName))]
		public long OwnerId { get; set; }

		/// <summary>
		/// 用户
		/// </summary>
		[TableVisible]
		[Ignore]
		public string OwnerName { get; set; }


	}
	public class TicketInternal : Ticket
	{
	}
	public class TicketEditable : Ticket
	{
		/// <summary>
		/// 内容
		/// </summary>
		[MultipleLines]
		public string Content { get; set; }

		/// <summary>
		/// 回复
		/// </summary>
		public IEnumerable<TicketReply> Replies { get; set; }

		/// <summary>
		/// 图片
		/// </summary>
		[ArrayLayout(HertMode =true)]
		[JsonData]
		public TicketImage[] Images { get; set; }
	}

	/// <summary>
	/// 工单分类
	/// </summary>
	[EntityObject]
	public class TicketReply: ObjectEntityBase
	{
		/// <summary>
		/// 回复人
		/// </summary>
		[EntityIdent(typeof(User), nameof(OwnerName))]
		public long OwnerId { get; set; }

		/// <summary>
		/// 回复人
		/// </summary>
		[TableVisible]
		[Ignore]
		public string OwnerName { get; set; }

		/// <summary>
		/// 管理员
		/// </summary>
		public bool IsAdmin { get; set; }

		/// <summary>
		/// 内容
		/// </summary>
		[MultipleLines]
		public string Content { get; set; }

		/// <summary>
		/// 图片
		/// </summary>
		[ArrayLayout(HertMode = true)]
		[JsonData]
		public TicketImage[] Images { get; set; }
	}
}
