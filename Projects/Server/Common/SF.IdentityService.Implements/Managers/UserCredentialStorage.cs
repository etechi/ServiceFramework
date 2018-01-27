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

using SF.Auth.IdentityServices.Internals;
using SF.Auth.IdentityServices.Models;
using SF.Sys;
using SF.Sys.Data;
using SF.Sys.Services;
using SF.Sys.TimeServices;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Auth.IdentityServices.Managers
{
	public class UserCredentialStorage :
		UserCredentialStorage<DataModels.User, DataModels.UserCredential, DataModels.UserClaimValue, DataModels.UserRole>
	{
		public UserCredentialStorage(IDataScope DataScope, ITimeService TimeService, IServiceInstanceDescriptor ServiceInstanceDescriptor) : base(DataScope, TimeService, ServiceInstanceDescriptor)
		{
		}
	}
	public class UserCredentialStorage<TUser, TUserCredential, TUserClaimValue,TUserRole> :
		IUserCredentialStorage
		where TUser:DataModels.User<TUser, TUserCredential, TUserClaimValue, TUserRole>
		where TUserCredential : DataModels.UserCredential<TUser, TUserCredential, TUserClaimValue, TUserRole>, new()
		where TUserClaimValue : DataModels.UserClaimValue<TUser, TUserCredential, TUserClaimValue, TUserRole>, new()
		where TUserRole : DataModels.UserRole<TUser, TUserCredential, TUserClaimValue, TUserRole>, new()
	{
		IDataScope DataScope { get; }
		ITimeService TimeService { get; }
		IServiceInstanceDescriptor ServiceInstanceDescriptor { get; }
		public UserCredentialStorage(
			IDataScope DataScope,
			ITimeService TimeService,
			IServiceInstanceDescriptor ServiceInstanceDescriptor
			)
		{
			this.DataScope = DataScope;
			this.TimeService = TimeService;
			this.ServiceInstanceDescriptor = ServiceInstanceDescriptor;
		}

		public Task<UserCredential> FindOrBind(string ClaimTypeId, string Credential, bool Confirmed, long UserId)
		{
			return DataScope.Use("查找或绑定用户", async ctx =>
			 {
				 var DataSet = ctx.Set<TUserCredential>();
				 var exist = await DataSet.FirstOrDefaultAsync(i => i.ClaimTypeId == ClaimTypeId && i.Credential == Credential);
				 var existUserId = exist?.UserId;

				 if (exist == null)
				 {
					 exist = DataSet.Add(new TUserCredential
					 {
						 ClaimTypeId = ClaimTypeId,
						 Credential = Credential,
						 UserId = existUserId ?? UserId,
						 CreatedTime = TimeService.Now,
						 ConfirmedTime = Confirmed ? (DateTime?)TimeService.Now : null
					 });
					 await DataSet.Context.SaveChangesAsync();
				 }
				 return Poco.Map<TUserCredential, UserCredential>(exist);
			 });
		}

		public Task<UserCredential> Find(string ClaimTypeId, string Credential)
		{
			return DataScope.Use("查找凭证", ctx => 
				ctx.Set<TUserCredential>()
				.FirstOrDefaultAsync(
				 i => i.ClaimTypeId == ClaimTypeId && i.Credential == Credential,
				 Poco.MapExpression<TUserCredential, UserCredential>()
				 ));
		}

		public Task Bind(string ClaimTypeId, string Credential, bool Confirmed, long UserId)
		{
			return DataScope.Use("查找凭证", ctx =>
			{
				ctx.Add(new TUserCredential
				{
					ClaimTypeId = ClaimTypeId,
					Credential = Credential,
					UserId = UserId,
					CreatedTime = TimeService.Now,
					ConfirmedTime = Confirmed ? (DateTime?)TimeService.Now : null
				});
				return ctx.SaveChangesAsync();
			});
		}

		public Task Unbind(string ClaimTypeId, string Credential, long UserId)
		{
			return DataScope.Use("解绑", ctx =>
				ctx.Set<TUserCredential>().RemoveRangeAsync(
					i => 
					i.ClaimTypeId == ClaimTypeId && 
					i.Credential == Credential && 
					i.UserId == UserId
					)
				);
		}

		public Task SetConfirmed(string ClaimTypeId, string Credential, bool Confirmed)
		{
			return DataScope.Use("设置验证状态", ctx =>
				ctx.Set<TUserCredential>().Update(
					i => i.ClaimTypeId == ClaimTypeId && i.Credential == Credential,
					e =>
					{
						e.ConfirmedTime = Confirmed ? (DateTime?)TimeService.Now : null;
					})
				);
		}

		public Task<UserCredential[]> GetIdents(string ClaimTypeId, long UserId)
		{
			return DataScope.Use("设置验证状态", ctx =>
				ctx.Set<TUserCredential>().QueryAsync(
				i => i.ClaimTypeId == ClaimTypeId && i.UserId == UserId,
				Poco.MapExpression<TUserCredential, UserCredential>()
				)
			);
		}

		public Task RemoveAllAsync()
		{
			return DataScope.Use("设置验证状态", ctx =>
				ctx.Set<TUserCredential>().RemoveRangeAsync(ic => true)
				);
		}
	}
	
}
