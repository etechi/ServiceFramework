﻿#region Apache License Version 2.0
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

namespace SF.Common.TextMessages.Management.DataModels
{
	[Table("CommonTextMessageActionRecords")]
	public class MsgActionRecord : EventEntityBase
	{
		/// <summary>
		/// 状态
		/// </summary>
		public SendStatus Status { get; set; }

		/// <summary>
		/// 发送服务
		/// </summary>
		[Index]
		public long ServiceId { get; set; }


		/// <summary>
		/// 接收方
		/// </summary>
		public string Target { get; set; }

		/// <summary>
		/// 发送方
		/// </summary>
		public string Sender { get; set; }

		/// <summary>
		/// 标题
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// 内容
		/// </summary>
		public string Content { get; set; }


		/// <summary>
		/// 消息参数
		/// </summary>
		public string Args { get; set; }


		/// <summary>
		/// 完成时间
		/// </summary>
		public DateTime? EndTime { get; set; }

		/// <summary>
		/// 错误信息
		/// </summary>
		public string Error { get; set; }

		/// <summary>
		/// 发送结果
		/// </summary>
		public string Result { get; set; }

		/// <summary>
		/// 跟踪对象
		/// </summary>
		[MaxLength(100)]
		[Index]
		public string TrackEntityId { get; set; }

		/// <summary>
		/// 消息记录
		/// </summary>
		[Index]
		public long MsgRecordId { get; set; }

		[ForeignKey(nameof(MsgRecordId))]
		public MsgRecord MsgRecord { get; set; }

	}


}
