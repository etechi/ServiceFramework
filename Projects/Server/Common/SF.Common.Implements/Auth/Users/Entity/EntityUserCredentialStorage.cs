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

using SF.Auth.Users.Internals;
using SF.Auth.Users.Models;
using SF.Core.ServiceManagement;
using SF.Core.Times;
using SF.Data;
using SF.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Auth.Users.Entity
{
	public class EntityUserCredentialStorage<TUser, TUserCredential, TUserClaimValue,TUserRole> :
		IUserCredentialStorage
		where TUser:DataModels.User<TUser, TUserCredential, TUserClaimValue, TUserRole>
		where TUserCredential : DataModels.UserCredential<TUser, TUserCredential, TUserClaimValue, TUserRole>, new()
		where TUserClaimValue : DataModels.UserClaimValue<TUser, TUserCredential, TUserClaimValue, TUserRole>, new()
		where TUserRole : DataModels.UserRole<TUser, TUserCredential, TUserClaimValue, TUserRole>, new()
	{
		IDataSet<TUserCredential> DataSet { get; }
		ITimeService TimeService { get; }
		IServiceInstanceDescriptor ServiceInstanceDescriptor { get; }
		public EntityUserCredentialStorage(
			IDataSet<TUserCredential> DataSet,
			ITimeService TimeService,
			IServiceInstanceDescriptor ServiceInstanceDescriptor
			)
		{
			this.DataSet = DataSet;
			this.TimeService = TimeService;
			this.ServiceInstanceDescriptor = ServiceInstanceDescriptor;
		}

		public async Task<UserCredential> FindOrBind(long Provider, string Credential, string UnionIdent, bool Confirmed, long UserId)
		{
			TUserCredential exist;
			long? existUserId;
			if (UnionIdent != null)
			{
				var es = await DataSet.QueryAsync(i => i.ProviderId == Provider && i.UnionIdent == UnionIdent);
				exist = es.First(i => i.Credential == Credential);
				existUserId = exist?.UserId ?? es.FirstOrDefault()?.UserId;
			}
			else
			{
				exist = await DataSet.FirstOrDefaultAsync(i => i.ProviderId == Provider && i.Credential == Credential);
				existUserId = exist?.UserId;
			}

			if (exist == null)
			{
				exist = DataSet.Add(new TUserCredential
				{
					ScopeId= ServiceInstanceDescriptor.DataScopeId??0,
					ProviderId = Provider,
					Credential = Credential,
					UserId = existUserId ?? UserId,
					UnionIdent = UnionIdent,
					CreatedTime = TimeService.Now,
					ConfirmedTime = Confirmed ? (DateTime?)TimeService.Now : null
				});
				await DataSet.Context.SaveChangesAsync();
			}
			return ADT.Poco.Map<TUserCredential, UserCredential>(exist);

		}

		public async Task<UserCredential> Find(long Provider, string Credential, string UnionIdent)
		{
			if (UnionIdent != null)
				return await DataSet.FirstOrDefaultAsync(
					i => i.ProviderId == Provider && i.UnionIdent == UnionIdent && i.Credential == Credential,
					ADT.Poco.MapExpression<TUserCredential, UserCredential>()
					);
			else
				return await DataSet.FirstOrDefaultAsync(
					i => i.ProviderId == Provider && i.Credential == Credential,
					ADT.Poco.MapExpression<TUserCredential, UserCredential>()
					);
		}

		public async Task Bind(long Provider, string Credential, string UnionIdent, bool Confirmed, long UserId)
		{
			DataSet.Add(new TUserCredential
			{
				ScopeId = ServiceInstanceDescriptor.DataScopeId.Value,
				ProviderId = Provider,
				Credential = Credential,
				UserId = UserId,
				UnionIdent = UnionIdent,
				CreatedTime = TimeService.Now,
				ConfirmedTime = Confirmed ? (DateTime?)TimeService.Now : null
			});
			await DataSet.Context.SaveChangesAsync();
		}

		public async Task Unbind(long Provider, string Credential, long UserId)
		{
			await DataSet.RemoveRangeAsync(i => i.ProviderId == Provider && i.Credential == Credential && i.UserId == UserId);
		}

		public async Task SetConfirmed(long Provider, string Credential, bool Confirmed)
		{
			await DataSet.Update(
				i =>i.ProviderId == Provider && i.Credential == Credential,
				e =>
				{
					e.ConfirmedTime = Confirmed ? (DateTime?)TimeService.Now : null;
				});
		}

		public async Task<UserCredential[]> GetIdents(long Provider, long UserId)
		{
			return await DataSet.QueryAsync(
				i => i.ProviderId == Provider && i.UserId == UserId,
				ADT.Poco.MapExpression<TUserCredential, UserCredential>()
				);
		}

		public async Task RemoveAllAsync()
		{
			var sid =  ServiceInstanceDescriptor.DataScopeId.Value;
			await DataSet.RemoveRangeAsync(ic => ic.ScopeId == sid);
		}
	}
	
}
