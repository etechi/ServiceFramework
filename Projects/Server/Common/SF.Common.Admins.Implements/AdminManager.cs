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



using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SF.Auth.IdentityServices.Managers;
using SF.Common.Admins.Models;
using SF.Sys;
using SF.Sys.Auth;
using SF.Sys.Entities;
using SF.Sys.Linq;
namespace SF.Common.Admins
{
	public class AdminManager
		: AdminManager<AdminInternal, AdminEditable, AdminQueryArgument>,
		IAdminManager
	{
		public AdminManager(
			IEntityServiceContext ServiceContext, 
			IUserManager UserManager,
			IRoleManager RoleManager
			)
			: base(ServiceContext, UserManager, RoleManager)
		{
		}
	}
	public class AdminManager<TInternal, TEditable, TQueryArgument> :
		IAdminManager<TInternal, TEditable, TQueryArgument>
		where TInternal : AdminInternal,new()
		where TEditable : AdminEditable,new()
		where TQueryArgument : AdminQueryArgument,new()
	{
		IEntityServiceContext ServiceContext { get; }
		IUserManager UserManager { get; }
		IRoleManager RoleManager { get; }
		public AdminManager(
			IEntityServiceContext ServiceContext, 
			IUserManager UserManager,
			IRoleManager RoleManager
			)
		{
			this.ServiceContext = ServiceContext;
			this.UserManager = UserManager;
			this.RoleManager = RoleManager;
		}

		public EntityManagerCapability Capabilities => EntityManagerCapability.All;

		public Task<TInternal[]> BatchGetAsync(ObjectKey<long>[] Ids, string[] Props)
		{
			throw new System.NotImplementedException();
		}

		public async Task<ObjectKey<long>> CreateAsync(TEditable Entity)
		{
			var re = await UserManager.CreateAsync(new Auth.IdentityServices.Models.UserEditable
			{
				Icon = Entity.Icon,
				Image = Entity.Image,
				InternalRemarks = Entity.InternalRemarks,
				LogicState = Entity.LogicState,
				Name=Entity.Name,
				MainClaimTypeId = SF.Sys.Auth.PredefinedClaimTypes.AdminAccount,
				MainCredential = Entity.UserName,
				Roles=Entity.Roles.Select(r=>new Auth.IdentityServices.Models.UserRole
				{
					RoleId=r
				}),
				PasswordHash=Entity.Password
			});
			return re;
		}

		public async Task<TInternal> GetAsync(ObjectKey<long> Id, string[] Fields = null)
		{
			var re = await UserManager.LoadForEdit(Id);
			if (re == null) return null;
			if (re.MainClaimTypeId != PredefinedClaimTypes.AdminAccount)
				throw new ArgumentException("指定对象不是管理员:" + Id.Id);

			var roles = await RoleManager.BatchGetAsync(
				re.Roles.Select(r => ObjectKey.From(r.RoleId)).ToArray(),
				null
				);
			var rnames = roles.Select(r => r.Name).Join(",");
			return new TInternal
			{
				Id = re.Id,
				CreatedTime = re.CreatedTime,
				Icon = re.Icon,
				Image = re.Image,
				InternalRemarks = re.InternalRemarks,
				LogicState = re.LogicState,
				Name = re.Name,
				RoleNames = rnames,
				UpdatedTime = re.UpdatedTime,
				UserName = re.MainCredential
			};
		}

		public async Task<TEditable> LoadForEdit(ObjectKey<long> Key)
		{
			var re = await UserManager.LoadForEdit(Key);
			if (re == null) return null;
			if (re.MainClaimTypeId != PredefinedClaimTypes.AdminAccount)
				throw new ArgumentException("指定对象不是管理员:" + Key.Id);

			var editable=new TEditable
			{
				Id = re.Id,
				CreatedTime = re.CreatedTime,
				Icon = re.Icon,
				Image = re.Image,
				InternalRemarks = re.InternalRemarks,
				LogicState = re.LogicState,
				Name = re.Name,
				UpdatedTime = re.UpdatedTime,
				UserName = re.MainCredential,
				Roles = re.Roles.Select(r => r.RoleId).ToArray()
			};
			return editable;
		}

		public async Task<QueryResult<TInternal>> QueryAsync(TQueryArgument Arg)
		{
			var ids = await QueryIdentsAsync(Arg);
			
			var items = new List<TInternal>();
			foreach(var id in ids.Items)
			{
				items.Add(await GetAsync(id));
			}
			return new QueryResult<TInternal>
			{
				Items = items,
				Total = ids.Total,
				Summary = ids.Summary
			};
		}

		public async Task<QueryResult<ObjectKey<long>>> QueryIdentsAsync(TQueryArgument Arg)
		{
			return await UserManager.QueryIdentsAsync(new UserQueryArgument
			{
				Id = Arg.Id,
				IsAdmin = true,
				LogicState = Arg.LogicState,
				Name = Arg.Name,
				MainCredential = Arg.UserName,
				Paging = Arg.Paging,
				MainClaimTypeId = PredefinedClaimTypes.AdminAccount,
				RoleId = Arg.RoleId
			});
		}

		public Task RemoveAllAsync()
		{
			throw new NotSupportedException();
		}

		public async Task RemoveAsync(ObjectKey<long> Key)
		{
			var u = await GetAsync(Key);
			if (u == null)
				return;
			await UserManager.RemoveAsync(Key);
		}

		public async Task UpdateAsync(TEditable Entity)
		{
			var u = await UserManager.LoadForEdit(ObjectKey.From(Entity.Id));
			if (u == null)
				throw new PublicArgumentException("找不到指定的管理员:" + Entity.Id);
			if(u.MainClaimTypeId!=PredefinedClaimTypes.AdminAccount)
				throw new PublicArgumentException("找不到指定的管理员:" + Entity.Id);
			u.Name = Entity.Name;
			u.Icon = Entity.Icon;
			u.PasswordHash = Entity.Password;
			u.Image = Entity.Image;
			u.Roles = Entity.Roles.Select(r => new Auth.IdentityServices.Models.UserRole
			{
				RoleId = r
			});
			u.LogicState = Entity.LogicState;
			u.InternalRemarks = Entity.InternalRemarks;

			await UserManager.UpdateAsync(u);
		}
	}

}
