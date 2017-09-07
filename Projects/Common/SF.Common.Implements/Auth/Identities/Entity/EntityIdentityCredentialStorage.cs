using SF.Auth.Identities.Internals;
using SF.Auth.Identities.Models;
using SF.Core.ServiceManagement;
using SF.Core.Times;
using SF.Data;
using SF.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Auth.Identities.Entity
{
	public class EntityIdentityCredentialStorage<TIdentity, TIdentityCredential> :
		IIdentityCredentialStorage
		where TIdentity:DataModels.Identity<TIdentity, TIdentityCredential>
		where TIdentityCredential : DataModels.IdentityCredential<TIdentity, TIdentityCredential>, new()
	{
		IDataSet<TIdentityCredential> DataSet { get; }
		ITimeService TimeService { get; }
		IServiceInstanceDescriptor ServiceInstanceDescriptor { get; }
		public EntityIdentityCredentialStorage(
			IDataSet<TIdentityCredential> DataSet,
			ITimeService TimeService,
			IServiceInstanceDescriptor ServiceInstanceDescriptor
			)
		{
			this.DataSet = DataSet;
			this.TimeService = TimeService;
			this.ServiceInstanceDescriptor = ServiceInstanceDescriptor;
		}

		public async Task<IdentityCredential> FindOrBind(long Provider, string Credential, string UnionIdent, bool Confirmed, long UserId)
		{
			TIdentityCredential exist;
			long? existUserId;
			if (UnionIdent != null)
			{
				var es = await DataSet.QueryAsync(i => i.ProviderId == Provider && i.UnionIdent == UnionIdent);
				exist = es.First(i => i.Credential == Credential);
				existUserId = exist?.IdentityId ?? es.FirstOrDefault()?.IdentityId;
			}
			else
			{
				exist = await DataSet.FirstOrDefaultAsync(i => i.ProviderId == Provider && i.Credential == Credential);
				existUserId = exist?.IdentityId;
			}

			if (exist == null)
			{
				exist = DataSet.Add(new TIdentityCredential
				{
					ScopeId= ServiceInstanceDescriptor.InstanceId,
					ProviderId = Provider,
					Credential = Credential,
					IdentityId = existUserId ?? UserId,
					UnionIdent = UnionIdent,
					CreatedTime = TimeService.Now,
					ConfirmedTime = Confirmed ? (DateTime?)TimeService.Now : null
				});
				await DataSet.Context.SaveChangesAsync();
			}
			return EntityMapper.Map<TIdentityCredential, IdentityCredential>(exist);

		}

		public async Task<IdentityCredential> Find(long Provider, string Credential, string UnionIdent)
		{
			if (UnionIdent != null)
				return await DataSet.FirstOrDefaultAsync(
					i => i.ProviderId == Provider && i.UnionIdent == UnionIdent && i.Credential == Credential,
					EntityMapper.Map<TIdentityCredential, IdentityCredential>()
					);
			else
				return await DataSet.FirstOrDefaultAsync(
					i => i.ProviderId == Provider && i.Credential == Credential,
					EntityMapper.Map<TIdentityCredential, IdentityCredential>()
					);
		}

		public async Task Bind(long Provider, string Credential, string UnionIdent, bool Confirmed, long UserId)
		{
			DataSet.Add(new TIdentityCredential
			{
				ScopeId = ServiceInstanceDescriptor.InstanceId,
				ProviderId = Provider,
				Credential = Credential,
				IdentityId = UserId,
				UnionIdent = UnionIdent,
				CreatedTime = TimeService.Now,
				ConfirmedTime = Confirmed ? (DateTime?)TimeService.Now : null
			});
			await DataSet.Context.SaveChangesAsync();
		}

		public async Task Unbind(long Provider, string Credential, long UserId)
		{
			await DataSet.RemoveRangeAsync(i => i.ProviderId == Provider && i.Credential == Credential && i.IdentityId == UserId);
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

		public async Task<IdentityCredential[]> GetIdents(long Provider, long UserId)
		{
			return await DataSet.QueryAsync(
				i => i.ProviderId == Provider && i.IdentityId == UserId,
				EntityMapper.Map<TIdentityCredential, IdentityCredential>()
				);
		}

		public async Task RemoveAllAsync()
		{
			var sid =  ServiceInstanceDescriptor.InstanceId;
			await DataSet.RemoveRangeAsync(ic => ic.ScopeId == sid);
		}
	}
	
}
