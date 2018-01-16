﻿using System;
using SF.Sys.Entities;

namespace SF.Common.Conversations.Front
{
	/// <summary>
	/// 会话消息查询
	/// </summary>
	public class MessageQueryArgument : QueryArgument
	{
		/// <summary>
		/// 业务标识类型
		/// </summary>
		public string BizIdentType { get; set; }

		/// <summary>
		/// 会话业务标识
		/// </summary>
		public long BizIdent { get; set; } 

		/// <summary>
		/// 若设置，则返回指定ID之后的记录,用于获取新增消息
		/// </summary>
		public long? StartId { get; set; }
	}
}