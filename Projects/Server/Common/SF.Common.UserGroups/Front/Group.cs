using SF.Sys.Entities;
using SF.Sys.Annotations;
using System;

namespace SF.Common.UserGroups.Front
{
	/// <summary>
	/// 用户组
	/// </summary>
	public class Group:IEntityWithId<long>
	{
		/// <summary>
		/// ID
		/// </summary>
		public long Id { get; set; }
		

		/// <summary>
		/// 用户组名称
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 用户组图标
		/// </summary>
		public string Icon { get; set; }
		
		/// <summary>
		/// 用户组标志
		/// </summary>
		public SessionFlag Flags { get; set; }

		/// <summary>
		/// 最后活动时间
		/// </summary>
		public DateTime LastActiveTime { get; set; }
		
		
		/// <summary>
		/// 成员数量
		/// </summary>
		public int MemberCount { get; set; }
		
		/// <summary>
		/// </summary>
		[EntityIdent(typeof(GroupMember))]
		public long? OwnerMemberId { get; set; }

		/// <summary>
		/// 所有人成员名称
		/// </summary>
		[FromEntityProperty(nameof(OwnerMemberId), nameof(GroupMember.Name))]
		public string OwnerMemberName { get; set; }

		/// <summary>
		/// 所有人成员图标
		/// </summary>
		[FromEntityProperty(nameof(OwnerMemberId), nameof(GroupMember.Icon))]
		public string OwnerMemberIcon { get; set; }

		/// <summary>
		/// 当前用户加入状态
		/// </summary>
		public GroupJoinState JoinState { get; set; }
	}
}	