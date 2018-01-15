using SF.Sys.Entities;

namespace SF.Common.Conversations.Front
{
	/// <summary>
	/// 会话成员查询参数
	/// </summary>
	public class MemberQueryArgument : QueryArgument
	{
		/// <summary>
		/// 会话ID
		/// </summary>
		public long? SessionId { get; set; }

		/// <summary>
		/// 用户ID
		/// </summary>
		public long? OwnerId { get; set; }
	}
}