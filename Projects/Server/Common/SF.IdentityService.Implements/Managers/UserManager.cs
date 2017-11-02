#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0

using SF.Core;
using SF.Core.ServiceManagement;
using SF.Core.Times;
using SF.Data;
using SF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SF.Auth.IdentityServices;
using SF.Auth.IdentityServices.Models;
using SF.Auth.IdentityServices.Internals;
using SF.Services.Security;

namespace SF.Auth.IdentityServices.Managers
{
	public class UserManager :
		EntityUserManager<UserInternal, UserEditable, UserQueryArgument, DataModels.User, DataModels.UserCredential, DataModels.UserClaimValue, DataModels.UserRole>,
		IUserManager
	{
		public UserManager(IEntityServiceContext ServiceContext, Lazy<IPasswordHasher> PasswordHasher) : base(ServiceContext, PasswordHasher)
		{
		}
	}
	public class EntityUserManager<TInternal,TEditable,TQueryArgument, TUser,TUserCredential,TUserClaimValue,TUserRole> :
		//QuerableEntitySource<long, Models.IdentityInternal, IdentityQueryArgument, TIdentity>,
		AutoModifiableEntityManager<
			ObjectKey<long>,
			TInternal,
			TInternal,
			TQueryArgument,
			TEditable,
			TUser
			>,
		IUserManager<TInternal, TEditable, TQueryArgument>,
		IUserStorage
		where TInternal : Models.UserInternal
		where TEditable : Models.UserEditable,new()
		where TQueryArgument : UserQueryArgument,new()
		where TUser :DataModels.User<TUser,TUserCredential, TUserClaimValue, TUserRole>,new()
		where TUserCredential : DataModels.UserCredential<TUser, TUserCredential, TUserClaimValue, TUserRole>,new()
		where TUserClaimValue : DataModels.UserClaimValue<TUser, TUserCredential, TUserClaimValue, TUserRole>, new()
		where TUserRole : DataModels.UserRole<TUser, TUserCredential, TUserClaimValue, TUserRole>, new()
	{
		Lazy<IPasswordHasher> PasswordHasher { get; }
		public EntityUserManager(IEntityServiceContext ServiceContext,Lazy<IPasswordHasher> PasswordHasher) : base(ServiceContext)
		{
			this.PasswordHasher = PasswordHasher;
		}

		async Task<long> IUserStorage.Create(UserCreateArgument Arg)
		{
			var roles = Arg.Roles == null ? null : Arg.Roles.Select(r => new UserRole { RoleId = r }).ToArray();

			var uid=await CreateAsync(new TEditable
			{
				Name = Arg.User.Name,
				Icon = Arg.User.Icon,

				MainCredential = Arg.CredentialValue,
				MainClaimTypeId = Arg.ClaimTypeId,

				LogicState = EntityLogicState.Enabled,
				SignupExtraArgument=Arg.ExtraArgument,
				PasswordHash = Arg.PasswordHash,
				SecurityStamp = Arg.SecurityStamp.Base64(),

				Roles= roles,
				//OwnerId = Arg.Identity.OwnerId,
				Credentials =new List<Models.UserCredential>
				{
					new Models.UserCredential
					{
						Credential=Arg.CredentialValue,
						ClaimTypeId=Arg.ClaimTypeId,
					}
				}
			});
			return uid.Id;

			//Ensure.NotNull(Arg.Identity, "身份标识");
			//Ensure.HasContent(Arg.Identity.Name, "身份标识名称");
			//Ensure.Positive(Arg.Identity.Id, "身份标识ID");
			//Ensure.HasContent(Arg.Identity.Entity, "身份类型");
			//Ensure.Positive(Arg.CredentialProvider, "未制定标识提供者");
			//Ensure.HasContent(Arg.CredentialValue, "未指定标识");
			//Ensure.NotNull(Arg.SecurityStamp, "未制定安全戳");
			

			//var time = TimeService.Value.Now;
			//DataSet.Add(new TIdentity
			//{
			//	//AppId =Arg.AppId,
			//	//ScopeId=Arg.ScopeId,
			//	CreatedTime= time,
			//	Id=Arg.Identity.Id,
			//	Name=Arg.Identity.Name,
			//	Icon=Arg.Identity.Icon,
			//	ObjectState=LogicObjectState.Enabled,
			//	PasswordHash=Arg.PasswordHash,
			//	SecurityStamp=Arg.SecurityStamp.Base64(),
			//	SignupIdentProviderId=Arg.CredentialProvider,
			//	SignupIdentValue=Arg.CredentialValue,
			//	UpdatedTime= time,
			//	Credentials=new[]
			//	{
			//		new TIdentityCredential
			//		{
			//			//AppId=Arg.AppId,
			//			//ScopeId=Arg.ScopeId,
			//			CreatedTime =time,
			//			ConfirmedTime=time,
			//			IdentityId=Arg.Identity.Id,
			//			Credential=Arg.CredentialValue,
			//			ProviderId=Arg.CredentialProvider,
						
			//		}
			//	}
			//});
		
			//await DataSet.Context.SaveChangesAsync();
			//return Arg.Identity.Id;
		}

		void PrepareCredentials(TEditable obj)
		{
			if (obj.Credentials == null || !obj.Credentials.Any())
				obj.Credentials = new[]{
					new Models.UserCredential
					{
						ClaimTypeId=obj.MainClaimTypeId,
						Credential=obj.MainCredential,
						BindTime=Now
					}
				};
		}
		public override Task<ObjectKey<long>> CreateAsync(TEditable obj)
		{
			if (obj.SecurityStamp.IsNullOrWhiteSpace())
			{
				var stamp = Bytes.Random(16);
				obj.PasswordHash = PasswordHasher.Value.Hash(obj.PasswordHash, stamp);
				obj.SecurityStamp = stamp.Base64();
			}
			PrepareCredentials(obj);
			return base.CreateAsync(obj);
		}
		public override Task UpdateAsync(TEditable obj)
		{
			PrepareCredentials(obj);
			return base.UpdateAsync(obj);
		}
		protected override async Task OnNewModel(IModifyContext ctx)
		{
			var e = ctx.Editable;
			var m = ctx.Model;
			
			await base.OnNewModel(ctx);
		}
		//async Task InitClaimTypes(TEditable obj)
		//{
		//	if (obj.Claims != null)
		//		foreach (var c in obj.Claims)
		//			if (c.TypeId == 0)
		//			{
		//				if (c.TypeName == null)
		//					throw new ArgumentException("类型错误");
		//				c.TypeId = await ServiceContext.GetOrCreateClaimType(c.TypeName);
		//			}
		//}
		//public override async Task<ObjectKey<long>> CreateAsync(TEditable obj)
		//{
		//	await InitClaimTypes(obj);
		//	return await base.CreateAsync(obj);
		//}
		//public override async Task UpdateAsync(TEditable obj)
		//{
		//	await InitClaimTypes(obj);
		//	await base.UpdateAsync(obj);
		//}
		protected override Task OnUpdateModel(IModifyContext ctx)
		{
			var e = ctx.Editable;
			var m = ctx.Model;
			return Task.CompletedTask;
			//if (e.Credentials != null)
			//{
			//	var ics = DataSet.Context.Set<TUserCredential>();
			//	var oitems = await ics.LoadListAsync(ic => ic.UserId == m.Id);
			//	ics.Merge(
			//		oitems,
			//		e.Credentials,
			//		(mi, ei) => mi.ClaimTypeId == ei.ClaimTypeId && mi.Credential== ei.Credential,
			//		ei => new TUserCredential
			//		{
			//			UserId = m.Id,
			//			Credential = ei.Credential,
			//			ClaimTypeId = ei.ClaimTypeId,
			//			CreatedTime = Now,
			//		}
			//		);
			//}
			//if (e.Roles != null)
			//{
			//	var irs = DataSet.Context.Set<TUserRole>();
			//	var oitems = await irs.LoadListAsync(ic => ic.UserId == m.Id);
			//	irs.Merge(
			//		oitems,
			//		e.Roles,
			//		(mi, ei) => mi.RoleId==ei.RoleId,
			//		ei => new TUserRole
			//		{
			//			UserId = m.Id,
			//			RoleId=ei.RoleId
			//		}
			//		);
			//}

			//if (e.Claims != null)
			//{
			//	var ccs = DataSet.Context.Set<TUserClaimValue>();
			//	var oids = ccs.AsQueryable(false).Where(c => c.UserId == m.Id).ToArray();

			//	foreach (var c in e.Claims)
			//	{
			//		if (c.Id == 0)
			//			c.Id = await IdentGenerator.GenerateAsync(typeof(TUserClaimValue).FullName);
			//		if(c.TypeId==0)
			//			c.TypeId = await ServiceContext.GetOrCreateClaimType(c.TypeName);
			//	}

			//	ccs.Merge(
			//		oids,
			//		e.Claims,
			//		(o, n) => o.Id == n.Id,
			//		n => new TUserClaimValue
			//		{
			//			Id = n.Id,
			//			TypeId = n.TypeId,
			//			UserId=m.Id,
			//			CreateTime = Now,
			//			UpdateTime = Now,
			//			Value = n.Value
			//		},
			//		(o, n) => {
			//			o.Value = n.Value;
			//			o.UpdateTime = Now;
			//		}
			//		);
			//}
		}
		
		
		protected override async Task OnRemoveModel(IModifyContext ctx)
		{
			//var ics = DataSet.Context.Set<TUserCredential>();
			//await ics.RemoveRangeAsync(ic => ic.UserId == ctx.Model.Id);

			//var cvs = DataSet.Context.Set<TUserClaimValue>();
			//await cvs.RemoveRangeAsync(ic => ic.UserId == ctx.Model.Id);

			await base.OnRemoveModel(ctx);
		}

		async Task<UserData> IUserStorage.Load(long Id)
		{
			var re=await DataSet.AsQueryable().Where(i => i.Id == Id)
				.Select(i => new 
			{
				stamp=i.SecurityStamp,
				data=new UserData
				{
					Id=i.Id,
					Icon=i.Icon,
					Name=i.Name,
					
					IsEnabled=i.LogicState==EntityLogicState.Enabled,
					PasswordHash=i.PasswordHash,
					Roles=i.Roles.Select(r=>r.RoleId),
					//Claims=i.Roles.SelectMany(r=>r.Role.ClaimValues.Select(cv=>new ClaimValue
					//{
					//	TypeId=cv.TypeId,
					//	Value=cv.Value
					//})).Union(
					//i.ClaimValues.Select(cv=>new ClaimValue
					//{
					//	TypeId = cv.TypeId,
					//	Value = cv.Value
					//}))
				}
			}).SingleOrDefaultAsync();
			if (re == null) return null;
			re.data.SecurityStamp = re.stamp.Base64();
			return re.data;
		}

		async Task IUserStorage.UpdateDescription(User Identity)
		{
			await DataSet.Update(Identity.Id, r =>
			{
				r.Name = Identity.Name;
				r.Icon = Identity.Icon;
			});
		}

		async Task IUserStorage.UpdateSecurity(long Id, string PasswordHash, byte[] SecurityStamp)
		{
			await DataSet.Update(Id, r =>
			{
				r.PasswordHash = PasswordHash;
				r.SecurityStamp = SecurityStamp.Base64();
			});
		}
	}

}
