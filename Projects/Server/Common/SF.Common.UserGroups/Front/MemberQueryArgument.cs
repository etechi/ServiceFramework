using SF.Sys.Entities;

namespace SF.Common.UserGroups.Front
{
	/// <summary>
	/// 成员查询参数
	/// </summary>
	public class MemberQueryArgument : QueryArgument
	{
		/// <summary>
		/// 组ID
		/// </summary>
		public long? GroupId { get; set; }

		/// <summary>
		/// 用户ID
		/// </summary>
		public long? OwnerId { get; set; }
	}
}