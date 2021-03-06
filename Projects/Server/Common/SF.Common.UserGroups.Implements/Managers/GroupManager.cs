﻿
using System;
using System.Linq;
using System.Threading.Tasks;
using SF.Common.UserGroups.Models;
using SF.Services.Security;
using SF.Sys;
using SF.Sys.Auth;
using SF.Sys.Data;
using SF.Sys.Entities;
using SF.Sys.Settings;
using SF.Sys.TimeServices;

namespace SF.Common.UserGroups.Managers
{
	public class GroupManager<TGroup, TMember,TGroupEditable,TQueryArgument,TDataGroup,TDataMember> :
		AutoModifiableEntityManager<
			ObjectKey<long>,
			TGroup,
			TGroup,
			TQueryArgument,
			TGroupEditable,
			TDataGroup
			>,
		IGroupManager<TGroup, TMember, TGroupEditable,TQueryArgument>
		where TGroup:Group<TGroup,TMember>,new()
		where TMember : GroupMember<TGroup, TMember>, new()
		where TGroupEditable:Group<TGroup, TMember>,TGroup
		where TQueryArgument:GroupQueryArgument,new()
		where TDataGroup:DataModels.DataGroup<TDataGroup,TDataMember>,new()
		where TDataMember : DataModels.DataGroupMember<TDataGroup, TDataMember>
	{
		public Lazy<IUserProfileService > UserProfileService { get; }

		public GroupManager(
			IEntityServiceContext ServiceContext,
			Lazy<IUserProfileService> UserProfileService,
			GroupSyncScope GroupSyncScope
			) : base(ServiceContext)
		{
			this.UserProfileService = UserProfileService;
			this.SetSyncQueue(GroupSyncScope, e => e.Id);
		}
		protected override async Task OnNewModel(IModifyContext ctx)
		{
			var editable = ctx.Editable;
			var model = ctx.Model;
			if (!editable.OwnerId.HasValue)
				throw new ArgumentException("必须指定用户组所有人");

			var user = await UserProfileService.Value.GetUser(editable.OwnerId.Value);
			if (user == null)
				throw new ArgumentException("找不到用户:" + editable.OwnerMemberId);

			if (editable.Name == null)
				editable.Name = user.Name + "的用户组";
			else
				editable.Name = string.Format(editable.Name, user.Name);

			if (editable.Icon == null)
				editable.Icon = user.Icon;
			model.Id = await IdentGenerator.GenerateAsync<TDataGroup>();
			model.LastActiveTime = Now;
			await base.OnNewModel(ctx);

			if (editable.Members == null)
				editable.Members = new[]{
					new TMember
					{
						GroupId=model.Id,
						OwnerId=user.Id,
						MemberAccepted=true,
						GroupAccepted=true,
						JoinState=GroupJoinState.Joined,
						LastActiveTime=Now
					}
				};

			var om = editable
				.Members
				.Where(mi=>mi.OwnerId==editable.OwnerId)
				.SingleOrDefault();
			if (om == null)
				throw new PublicArgumentException("缺少用户组所有人");

			if (om.Name.IsNullOrEmpty())
				om.Name = user.Name;
			if (om.Icon.IsNullOrEmpty())
				om.Icon = user.Icon;
		}
		protected override async Task OnUpdateModel(IModifyContext ctx)
		{
			await base.OnUpdateModel(ctx);

			if (ctx.Editable.Members != null)
			{
                foreach (var m in ctx.Model.Members)
                {
                    var newJoinState = JoinStateDetector.Detect(m.GroupAccepted, m.MemberAccepted);
                    if (newJoinState == GroupJoinState.Joined && m.JoinState!= GroupJoinState.Joined)
                        m.LastJoinTime = Now;
                    m.JoinState = newJoinState;
                    
                }
				ctx.Model.MemberCount =
					ctx.Model.Members
						.Count(mi => mi.LogicState == EntityLogicState.Enabled && mi.JoinState==GroupJoinState.Joined );
			}

			if (ctx.Action == ModifyAction.Create)
			{
				foreach (var m in ctx.Model.Members)
					m.LastActiveTime = Now;

				//由于循环引用，不能直接设置OwnerMemberId，需要先保存，然后重新设置
				//由数据库事务确保原子性
				await ctx.DataContext.SaveChangesAsync();

				ctx.DataContext.Update(ctx.Model);


				ctx.Model.OwnerMemberId = ctx.Model.Members.First().Id;

			}
			
		}
		

	}

}
