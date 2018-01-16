using SF.Sys.Entities;
using SF.Sys.Entities.Models;
using SF.Sys.Annotations;
using SF.Sys.Auth;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SF.Common.UserGroups.Models
{
	/// <summary>
	/// 用户组成员
	/// </summary>
	public class GroupMember<TGroup,TMember> : ObjectEntityBase
		where TGroup:Group<TGroup,TMember>
		where TMember : GroupMember<TGroup, TMember>
	{
		/// <summary>
		/// 成员图标
		/// </summary>
		public string Icon { get; set; }

		/// <summary>
		/// 用户组
		/// </summary>
		//[EntityIdent(null, nameof(GroupName))]
		public virtual long GroupId { get; set; }

		/// <summary>
		/// 用户组
		/// </summary>
		[TableVisible]
		[Ignore]
		public string GroupName { get; set; }

		/// <summary>
		/// 用户
		/// </summary>
		[EntityIdent(typeof(User), nameof(OwnerName))]
		[Uneditable]
		public long? OwnerId { get; set; }

		/// <summary>
		/// 用户
		/// </summary>
		[TableVisible]
		[Ignore]
		public string OwnerName { get; set; }

		/// <summary>
		/// 是否接收成员
		/// </summary>
		public bool? GroupAccepted { get; set; }

		/// <summary>
		/// 是否同意加入
		/// </summary>
		public bool? MemberAccepted { get; set; }

		/// <summary>
		/// 加入状态
		/// </summary>
		[ReadOnly(true)]
		[TableVisible]
		public GroupJoinState JoinState { get; set; }


		/// <summary>
		/// 最后活跃时间
		/// </summary>
		[ReadOnly(true)]
		[TableVisible]
		public DateTime LastActiveTime{get;set;}
	
	}
}