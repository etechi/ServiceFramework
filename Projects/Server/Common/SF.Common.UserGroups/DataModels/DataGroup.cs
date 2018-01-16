using SF.Sys.Entities;
using SF.Sys.Annotations;
using SF.Sys.Entities.DataModels;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SF.Sys.Data;
using System.Collections.Generic;
using System;

namespace SF.Common.UserGroups.DataModels
{
	/// <summary>
	/// 用户组
	/// </summary>
	[Table("UserGroup")]
	public class DataGroup<TGroup,TMember> : DataObjectEntityBase
		where TGroup:DataGroup<TGroup,TMember>
		where TMember: DataGroupMember<TGroup, TMember>
	{
		/// <summary>
		/// 用户组图标
		/// </summary>
		[MaxLength(100)]
		public string Icon { get; set; }

		/// <summary>
		/// 所有人ID
		/// </summary>
		[Index]
		public override long? OwnerId { get; set; }
		/// <summary>
		/// 用户组标志
		/// </summary>
		public SessionFlag Flags { get; set; }

	
		/// <summary>
		/// 成员数量
		/// </summary>
		public int MemberCount { get; set; }

		/// <summary>
		/// 所有成员ID
		/// </summary>
		[Index]
		public long? OwnerMemberId { get; set; }

		[ForeignKey(nameof(OwnerMemberId))]
		public TMember OwnerMember { get; set; }

		public DateTime LastActiveTime { get; set; }
		
		
		/// <summary>
		/// 用户组成员
		/// </summary>
		[InverseProperty(nameof(DataGroupMember<TGroup,TMember>.Group))]
		public ICollection<TMember> Members { get; set; }

		
	}
}