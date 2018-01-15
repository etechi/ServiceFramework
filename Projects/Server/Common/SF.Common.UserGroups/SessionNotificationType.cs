namespace SF.Common.Conversations
{
	/// <summary>
	/// 会话成员加入状态
	/// </summary>
	public enum SessionJoinState
	{
		/// <summary>
		/// 未知状态
		/// </summary>
		None,
		/// <summary>
		/// 邀请中
		/// </summary>
		Inviting,
		/// <summary>
		/// 申请中
		/// </summary>
		Applying,
		/// <summary>
		/// 拒绝邀请
		/// </summary>
		InviteRejected,
		/// <summary>
		/// 拒绝申请
		/// </summary>
		ApplyRejected,
		/// <summary>
		/// 已加入
		/// </summary>
		Joined,
	}
}	