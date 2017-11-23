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
		public UserCredentialStorage(IDataSet<DataModels.UserCredential> DataSet, ITimeService TimeService, IServiceInstanceDescriptor ServiceInstanceDescriptor) : base(DataSet, TimeService, ServiceInstanceDescriptor)
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
		IDataSet<TUserCredential> DataSet { get; }
		ITimeService TimeService { get; }
		IServiceInstanceDescriptor ServiceInstanceDescriptor { get; }
		public UserCredentialStorage(
			IDataSet<TUserCredential> DataSet,
			ITimeService TimeService,
			IServiceInstanceDescriptor ServiceInstanceDescriptor
			)
		{
			this.DataSet = DataSet;
			this.TimeService = TimeService;
			this.ServiceInstanceDescriptor = ServiceInstanceDescriptor;
		}

		public async Task<UserCredential> FindOrBind(string ClaimTypeId, string Credential, bool Confirmed, long UserId)
		{
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

		}

		public async Task<UserCredential> Find(string ClaimTypeId, string Credential)
		{
			return await DataSet.FirstOrDefaultAsync(
				i => i.ClaimTypeId == ClaimTypeId && i.Credential == Credential,
				Poco.MapExpression<TUserCredential, UserCredential>()
				);
		}

		public async Task Bind(string ClaimTypeId, string Credential, bool Confirmed, long UserId)
		{
			DataSet.Add(new TUserCredential
			{
				Credential = Credential,
				UserId = UserId,
				CreatedTime = TimeService.Now,
				ConfirmedTime = Confirmed ? (DateTime?)TimeService.Now : null
			});
			await DataSet.Context.SaveChangesAsync();
		}

		public async Task Unbind(string ClaimTypeId, string Credential, long UserId)
		{
			await DataSet.RemoveRangeAsync(i => i.ClaimTypeId == ClaimTypeId && i.Credential == Credential && i.UserId == UserId);
		}

		public async Task SetConfirmed(string ClaimTypeId, string Credential, bool Confirmed)
		{
			await DataSet.Update(
				i =>i.ClaimTypeId == ClaimTypeId && i.Credential == Credential,
				e =>
				{
					e.ConfirmedTime = Confirmed ? (DateTime?)TimeService.Now : null;
				});
		}

		public async Task<UserCredential[]> GetIdents(string ClaimTypeId, long UserId)
		{
			return await DataSet.QueryAsync(
				i => i.ClaimTypeId == ClaimTypeId && i.UserId == UserId,
				Poco.MapExpression<TUserCredential, UserCredential>()
				);
		}

		public async Task RemoveAllAsync()
		{
			await DataSet.RemoveRangeAsync(ic => true);
		}
	}
	
}
