﻿
using System.Threading.Tasks;
using SF.Sys.Entities;
using SF.Sys.Auth;
using System;
using SF.Sys.Data;
using System.Linq;
using SF.Sys.Clients;
using SF.Sys;
using SF.Sys.TimeServices;
using System.Collections.Generic;

namespace SF.Common.UserGroups.Front
{
	public class UserGroupServiceSetting<
		TGroup,
		TGroupEditable,
		TGroupQueryArgument,
		TGroupManager,
		TMember,
		TMemberEditable,
		TMemberQueryArgument,
		TGroupMemberManager
		>
		where TGroup : Models.Group<TGroup, TMember>
		where TGroupEditable : Models.Group<TGroup, TMember>, TGroup
		where TGroupQueryArgument : Managers.GroupQueryArgument
		where TMember : Models.GroupMember<TGroup, TMember>
		where TMemberEditable : Models.GroupMember<TGroup, TMember>
		where TMemberQueryArgument : Managers.GroupMemberQueryArgument
		where TGroupManager: Managers.IGroupManager<TGroup, TMember, TGroupEditable, TGroupQueryArgument>
		where TGroupMemberManager : Managers.IGroupMemberManager<TGroup, TMember, TMemberEditable, TMemberQueryArgument>
	{
		public IDataScope DataScope { get; set; }
		public IAccessToken AccessToken { get; set; }
		public Lazy<TGroupManager> GroupManager { get; set; }
		public Lazy<TGroupMemberManager> GroupMemberManager { get; set; }
		public Lazy<ITimeService> TimeService { get; set; }
		public Lazy<IUserProfileService> UserProfileService { get; set; }
	}
	/// <summary>
	/// 用户组服务
	/// </summary>
	public class UserGroupService<
		TFrontGroup,
		TFrontMember,
		TGroup,
		TGroupEditable, 
		TGroupQueryArgument,
		TGroupManager,
		TMember,
		TMemberEditable,
		TMemberQueryArgument,
		TGroupMemberManager,
		TDataGroup,
		TDataMember
		> : 
		IUserGroupService<TFrontGroup,TFrontMember>
		where TFrontGroup:Group,new()
		where TFrontMember:GroupMember,new()
		where TGroup:Models.Group<TGroup,TMember>
		where TGroupEditable : Models.Group<TGroup, TMember>, TGroup
		where TGroupQueryArgument:Managers.GroupQueryArgument
		where TMember : Models.GroupMember<TGroup, TMember>
		where TMemberEditable: Models.GroupMember<TGroup,TMember>
		where TMemberQueryArgument:Managers.GroupMemberQueryArgument
		where TDataGroup : DataModels.DataGroup<TDataGroup, TDataMember>
		where TDataMember : DataModels.DataGroupMember<TDataGroup, TDataMember>
		where TGroupManager : Managers.IGroupManager<TGroup, TMember, TGroupEditable, TGroupQueryArgument>
		where TGroupMemberManager : Managers.IGroupMemberManager<TGroup, TMember, TMemberEditable, TMemberQueryArgument>
	{
		protected UserGroupServiceSetting<TGroup,TGroupEditable,TGroupQueryArgument,TGroupManager,TMember,TMemberEditable,TMemberQueryArgument, TGroupMemberManager> Setting { get; }
		public long EnsureUserIdent() =>
			Setting.AccessToken.User.EnsureUserIdent();


		public UserGroupService(
			UserGroupServiceSetting<TGroup, TGroupEditable, TGroupQueryArgument, TGroupManager, TMember, TMemberEditable, TMemberQueryArgument, TGroupMemberManager> Setting
			)
		{
			this.Setting = Setting;
		}
		async Task<TDataMember> EnsureGroupMember(IDataContext DataContext,long SessionId, long UserId)
		{
			var member = await DataContext
				.Set<TDataMember>()
				.AsQueryable()
				.Where(m =>
					m.GroupId == SessionId &&
					m.OwnerId == UserId &&
					m.LogicState == EntityLogicState.Enabled &&
					m.JoinState == GroupJoinState.Joined
					)
				.SingleOrDefaultAsync();
			if (member == null)
				throw new PublicDeniedException("您已经离开用户组");
			return member;
		}

		

		/// <summary>
		/// 查询用户组成员
		/// </summary>
		/// <param name="Arg">查询参数</param>
		/// <returns></returns>
		public Task<QueryResult<TFrontMember>> QueryMembers(MemberQueryArgument Arg)
		{
			var user = EnsureUserIdent();
			return Setting.DataScope.Use("查询组成员", async DataContext =>
			{

				var q = DataContext.Set<TDataMember>().AsQueryable();
				if (Arg.Id != null)
				{
					var id = Arg.Id.Id;
					q = q.Where(m => m.Id == id && m.OwnerId == user && m.LogicState == EntityLogicState.Enabled);
				}
				else
				{
					if (!Arg.GroupId.HasValue && !Arg.OwnerId.HasValue)

						return QueryResult<TFrontMember>.Empty;
					q = q.Where(m => m.LogicState == EntityLogicState.Enabled);
					if (Arg.GroupId.HasValue)
					{
						var cid = Arg.GroupId.Value;
						await EnsureGroupMember(DataContext,cid, user);
						q = q.Where(m => m.GroupId == cid);
					}
					if (Arg.OwnerId.HasValue)
						q = q.Where(m =>
							m.OwnerId == Arg.OwnerId
							);
					q = q.OrderByDescending(m => m.LastActiveTime);
				}

				var rq = from m in q

						 select new TFrontMember
						 {
							 Id = m.Id,
							 GroupId = m.GroupId,
							 Name = m.Name,
							 Icon = m.Icon,
							 LastActiveTime = m.LastActiveTime,
							 JoinState = m.JoinState,
						 };
				var re = await rq.ToQueryResultAsync(Arg.Paging);
				return re;
			});
		}

		/// <summary>
		/// 查询用户组
		/// </summary>
		/// <param name="Arg">查询参数</param>
		/// <returns></returns>
		public Task<QueryResult<TFrontGroup>> QueryGroups(GroupQueryArgument Arg)
		{
			var user = EnsureUserIdent();
			return Setting.DataScope.Use("查询组", async DataContext =>
			{
				var q = Arg.Id != null ?
				//查询指定用户组，必须是用户组成员
				from s in DataContext.Set<TDataGroup>().AsQueryable()
				where s.Id == Arg.Id.Id &&
						s.LogicState == EntityLogicState.Enabled
				let sm = s.Members.FirstOrDefault(m => m.OwnerId.Value == user)
				where sm != null
				select new { s, sm }
			:
				//查询所有自己参加的用户组
				from sm in DataContext.Set<TDataMember>().AsQueryable()
				where sm.OwnerId == user &&
						sm.LogicState == EntityLogicState.Enabled
				let s = sm.Group
				where s.LogicState == EntityLogicState.Enabled
				orderby s.LastActiveTime descending
				select new { s, sm }
				;
				var rq = from i in q
						 let s = i.s
						 let sm = i.sm
						 select new TFrontGroup
						 {
							 Id = s.Id,
							 Name = s.Name,
							 Icon = s.Icon,
							 Flags = s.Flags,
							 MemberCount = s.MemberCount,
							 LastActiveTime = s.LastActiveTime,
							 OwnerMemberId = s.OwnerMemberId,
							 OwnerMemberIcon = s.OwnerMember.Icon,
							 OwnerMemberName = s.OwnerMember.Name,
							 JoinState = sm.JoinState,

						 };
				var re = await rq.ToQueryResultAsync(Arg.Paging);
				return re;
			});
		}


		async Task RemoveMember(TDataMember member)
		{
			await Setting.GroupMemberManager.Value.RemoveAsync(ObjectKey.From(member.Id));
		}
		/// <summary>
		/// 将某人从自己的用户组中移除
		/// </summary>
		/// <param name="MemberId">成员ID</param>
		/// <returns></returns>
		public  Task RemoveMember(long MemberId)
		{
			var user = EnsureUserIdent();
			return Setting.DataScope.Use("删除组成员", async DataContext =>
			{
				var member = await (
				from m in DataContext.Set<TDataMember>().AsQueryable()
				where m.Id == MemberId && m.Group.OwnerId == user && m.LogicState == EntityLogicState.Enabled
				select m
				).SingleOrDefaultAsync();
				if (member == null)
					throw new PublicArgumentException("找不到指定的成员");

				await RemoveMember(member);
			});
		}

		/// <summary>
		/// 离开某个已加入的用户组
		/// </summary>
		/// <param name="GroupId">用户组ID</param>
		/// <returns></returns>
		public Task LeaveGroup(long GroupId)
		{
			var user = EnsureUserIdent();
			return Setting.DataScope.Use("删除成员", async DataContext =>
			{

				var member = await EnsureGroupMember(DataContext,GroupId, user);
				await RemoveMember(member);
			});
		}

		public async Task UpdateMember(TFrontMember Member)
		{
			var user = EnsureUserIdent();


			var editable = await Setting.GroupMemberManager.Value.LoadForEdit(ObjectKey.From(Member.Id));
			if (editable == null)
				throw new PublicArgumentException("找不到指定的成员");

			//只有用户组成员用户或用户组所有人能修改成员信息
			if (editable.OwnerId != user)
			{
				await Setting.DataScope.Use("确认权限", async DataContext =>
				{
					if (!await DataContext
						.Set<TDataGroup>()
						.AsQueryable()
						.AnyAsync(c => c.Id == editable.GroupId && c.OwnerId.Value == user))
						throw new PublicDeniedException("您不能修改此成员信息");
				});
			}

			editable.Name = Member.Name;
			editable.Icon = Member.Icon;
			await Setting.GroupMemberManager.Value.UpdateAsync(editable);

		}


		public async Task UpdateGroup(TFrontGroup Session)
		{
			var user = EnsureUserIdent();
			var c = await Setting.GroupManager.Value.LoadForEdit(ObjectKey.From(Session.Id));
			if (c == null)
				throw new PublicArgumentException("找不到用户组:"+Session.Id);
			if (c.OwnerId.Value != user)
				throw new PublicDeniedException("只能修改自己的用户组信息");
			c.Name = Session.Name;
			c.Icon = Session.Icon;
			await Setting.GroupManager.Value.UpdateAsync(c);
		}

		

		public async Task SetAcceptType(long MemberId, bool AcceptType)
		{
			var uid = EnsureUserIdent();
			var m = await Setting.GroupMemberManager.Value.LoadForEdit(ObjectKey.From(MemberId));
			if (uid == m.OwnerId)
			{
				m.MemberAccepted = AcceptType;
				await Setting.GroupMemberManager.Value.UpdateAsync(m);
			}
			else
			{
				await Setting.DataScope.Use("确认权限", async DataContext =>
				{
					var soid = await DataContext.Set<TDataGroup>().AsQueryable().Where(s => s.Id == m.GroupId).Select(s => s.OwnerId).SingleOrDefaultAsync();
					if (uid != soid)
						throw new PublicDeniedException("当前用户必须是用户组所有者或成员所有者");
				});
				m.GroupAccepted = AcceptType;
				await Setting.GroupMemberManager.Value.UpdateAsync(m);

			}
		}
	}
}