using System;
using SF.Sys.Entities;

namespace SF.Common.Conversations.Front
{
	/// <summary>
	/// 会话成员查询参数
	/// </summary>
	public class MemberQueryArgument : QueryArgument
	{
		/// <summary>
		/// 业务标识类型
		/// </summary>
		public string BizIdentType { get; set; }

		/// <summary>
		/// 会话业务标识
		/// </summary>
		public long BizIdent { get; set; }


	}
}