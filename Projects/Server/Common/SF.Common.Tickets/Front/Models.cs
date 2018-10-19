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

namespace SF.Common.Tickets.Front
{
    /// <summary>
    /// 工单创建参数
    /// </summary>
    public class TicketCreateArgument
    {
        
        /// <summary>
        /// 分类
        /// </summary>
        public long CategoryId { get; set; }

        /// <summary>
        /// 工单标题
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 工单内容,Html格式
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 工单图片
        /// </summary>
        public TicketImage[] Images { get; set; }

    }
    public class TicketReplyCreateArgument
    {
        /// <summary>
        /// 工单ID
        /// </summary>
        public long TicketId { get; set; }

        /// <summary>
        /// 回复标题
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 回复内容,Html格式
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 回复图片
        /// </summary>
        public TicketImage[] Images { get; set; }
    }


    /// <summary>
    /// 工单
    /// </summary>
    public class Ticket : ObjectEntityBase
	{
        
        /// <summary>
        /// 分类
        /// </summary>
        public long CategoryId { get; set; }

		/// <summary>
		/// 分类名
		/// </summary>
		public string CategoryName { get; set; }

		/// <summary>
		/// 工单内容,Html格式
		/// </summary>
		public string Content { get; set; }

		/// <summary>
		/// 工单图片
		/// </summary>
		public TicketImage[] Images { get; set; }

		public string ImageStr { get; set; }
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
		/// 工单ID
		/// </summary>
		public long TicketId { get; set; }

		/// <summary>
		/// 回复人
		/// </summary>
		public long? OwnerId { get; set; }

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

		public string ImageStr { get; set; }
	}
	
}
