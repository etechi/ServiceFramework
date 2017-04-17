using SF.Data.Storage;
using SF.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using SF.Clients;
using SF.Data.Entity;
using SF.Core.Times;
using SF.Auth.Identity.DataModels;
using SF.Auth.Identity.Models;
using SF.Auth.Identity.Internals;
using SF.KB;

namespace SF.Auth.Identity
{
	public class EntityUserAdminService<TDataModel> :
		SF.Data.Entity.EntityManager<long,MemberInternal, UserQueryArgument,UserEditable, TDataModel>,
		IMemberManagementService,
		IUserStorage
		where TDataModel:DataModels.User,new()
	{
		protected override PagingQueryBuilder<TDataModel> PagingQueryBuilder => new PagingQueryBuilder<TDataModel>(
				"time",
				b => b.Add("time", i => i.CreatedTime, true));
		public ITimeService TimeService { get; }

		public EntityUserAdminService(
			IDataSet<TDataModel> DataSet,
			ITimeService TimeService
			) :base(DataSet)
		{
			this.TimeService = TimeService;
		}
		protected virtual UserInfo OnBuildUserInfo(MemberInternal u)
		{
			return new UserInfo
			{
				Id = u.Id,
				Icon = u.Icon,
				Image = u.Image,
				NickName = u.NickName,
				Sex = u.Sex,
				//Type = u.Type
			};
		}
		async Task<UserInfo> IUserStorage.FindById(long UserId)
		{
			var u = await GetAsync(UserId);
			return u == null ? null : OnBuildUserInfo(u);
		}

		async Task IUserStorage.UpdateAsync(UserInfo User)
		{
			await this.UpdateEntity(
				User.Id,
				e =>
				{
					e.Icon = User.Icon ?? e.Icon;
					e.Image = User.Image ?? e.Image;
					e.NickName = User.NickName ?? e.NickName;
					e.Sex = User.Sex ?? e.Sex;
				});
		}

		async Task<long> IUserStorage.Create(IdentCreateArgument Arg)
		{
			return await CreateAsync(new UserEditable
			{
				NickName =Arg.User.NickName,
				Icon=Arg.User.Icon,
				Image=Arg.User.Image,
				Sex =Arg.User.Sex ??SexType.Unknown,
				//Type =Arg.User.Type
			});
		}

		Task<string> IUserStorage.GetPasswordHash(long UserId, bool ForSignin)
		{
			if (ForSignin)
			{
				var now = TimeService.Now;
				return DataSet.QuerySingleAsync(
					e => 
						e.Id == UserId && 
						(e.LockoutEndDate==null || e.LockoutEndDate<now) &&
						e.ObjectState==LogicObjectState.Enabled, 
						e => e.PasswordHash
						);
			}
			else
				return DataSet.QuerySingleAsync(e => e.Id == UserId, e => e.PasswordHash);
		}

		Task IUserStorage.SetPasswordHash(long UserId, string PasswordHash, string SecurityStamp)
		{
			return DataSet.Update(
				UserId,
				e =>
				{
					e.PasswordHash = PasswordHash;
					e.SecurityStamp = SecurityStamp;
				});
		}

		protected override Task OnUpdateModel(ModifyContext ctx)
		{
			var e = ctx.Editable;
			var m = ctx.Model;
			m.NickName = e.NickName;
			m.UserName = e.UserName;
			m.Image = e.Image;
			m.Icon = e.Icon;
			m.UpdatedTime = TimeService.Now;
			return Task.CompletedTask;		
		}
		protected override Task OnNewModel(ModifyContext ctx)
		{
			ctx.Model.CreatedTime = TimeService.Now;
			return base.OnNewModel(ctx);
		}


		protected override IContextQueryable<TDataModel> OnBuildQuery(IContextQueryable<TDataModel> Query, UserQueryArgument Arg, Paging paging)
		{
			return Query.Filter(Arg.NickName, e => e.NickName);
		}

	

		async Task IUserStorage.SigninSuccess(long UserId, AccessInfo AccessInfo)
		{
			var re = await DataSet.Update(
				UserId,
				e =>
				{
					e.AccessFailedCount = 0;
					e.LockoutEndDate = null;
					e.LastAddress = AccessInfo.ClientAddress;
					//e.LastDeviceType=AccessInfo.
					e.LastSigninTime = TimeService.Now;
				}
				);
		}

		async Task IUserStorage.SigninFailed(long UserId, int LockoutFailedCount, TimeSpan LockoutTime, AccessInfo AccessInfo)
		{
			await DataSet.Update(
				UserId,
				e =>
				{
					e.AccessFailedCount++;
					if (e.AccessFailedCount > LockoutFailedCount)
						e.LockoutEndDate = TimeService.Now.Add(LockoutTime);
				}
				);
		}
	}

	public class EntityUserAdminService :
		EntityUserAdminService<DataModels.User>
	{
		public EntityUserAdminService(IDataSet<User> DataSet, ITimeService TimeService) : base(DataSet, TimeService)
		{
		}
	}
}
