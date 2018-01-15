using SF.Sys.Entities;
using SF.Sys.Annotations;
using SF.Sys.Entities.Models;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using SF.Sys.Auth;
using System.ComponentModel.DataAnnotations;

namespace SF.Common.UserGroups.Models
{
	/// <summary>
	/// 交谈用户组
	/// </summary>
	public class Group<TGroup, TMember> : ObjectEntityBase
		where TGroup:Group<TGroup,TMember>
		where TMember:GroupMember<TGroup,TMember>
	{
		/// <summary>
		/// 图标
		/// </summary>
		public string Icon { get; set; }
		
		/// <summary>
		/// 成员数量
		/// </summary>
		[ReadOnly(true)]
		[TableVisible]
		public int MemberCount { get; set; }
		
		/// <summary>
		/// 最后活跃时间
		/// </summary>
		[ReadOnly(true)]
		[TableVisible]
		public DateTime LastActiveTime { get; set; }

		/// <summary>
		/// 所有人
		/// </summary>
		[EntityIdent(typeof(User), nameof(OwnerName))]
		[Uneditable]
		public long? OwnerId { get; set; }

		/// <summary>
		/// 所有人
		/// </summary>
		[TableVisible]
		[Ignore]
		public string OwnerName { get; set; }

		/// <summary>
		/// 用户组标志
		/// </summary>
		public SessionFlag Flags { get; set; }
		/// <summary>
		/// 所有人成员
		/// </summary>
		[ReadOnly(true)]
		public virtual long? OwnerMemberId { get; set; }

		/// <summary>
		/// 所有人成员
		/// </summary>
		[TableVisible]
		[Ignore]
		public string OwnerMemberName { get; set; }

		
	}

	public class GroupEditable<TGroup,TMember> : Group<TGroup,TMember>
		where TMember:GroupMember<TGroup,TMember>
		where TGroup : Group<TGroup, TMember>
	{
		/// <summary>
		/// 成员
		/// </summary>
		[Ignore]
		public IEnumerable<TMember> Members { get; set; }
	}
}