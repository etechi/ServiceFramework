using SF.Sys.Entities;
using SF.Sys.Entities.DataModels;
using SF.Sys.Annotations;
using SF.Sys.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.ComponentModel.DataAnnotations;

namespace SF.Common.UserGroups.DataModels
{
	/// <summary>
	/// 用户组成员
	/// </summary>
	[Table("UserGroupMember")]
	public class DataGroupMember<TGroup, TMember> : DataObjectEntityBase
		where TGroup : DataGroup<TGroup, TMember>
		where TMember : DataGroupMember<TGroup, TMember>
	{
		/// <summary>
		/// 用户组
		/// </summary>
		[Index("member",IsUnique =true,Order =1)]
		[Index("session", Order = 1)]
		public long GroupId { get; set; }

		[ForeignKey(nameof(GroupId))]
		public TGroup Group { get; set; }

		/// <summary>
		/// 用户
		/// </summary>
		[Index]
		[Index("member", IsUnique = true, Order = 2)]
		public override long? OwnerId { get; set; }
		
		/// <summary>
		/// 成员图标
		/// </summary>
		public string Icon { get; set; }

		/// <summary>
		/// 成员最后活跃时间
		/// </summary>
		[Index("session", Order = 2)]
		public DateTime LastActiveTime { get; set; }
	

		/// <summary>
		/// 用户组所有者是否同意加入
		/// </summary>
		public bool? SessionAccepted { get; set; }

		/// <summary>
		/// 用户是否同意加入
		/// </summary>
		public bool? MemberAccepted { get; set; }

		/// <summary>
		/// 加入状态
		/// </summary>
		public GroupJoinState JoinState { get; set; }

		
	}
}