using SF.Data.Storage;
using SF.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using SF.Clients;
using SF.Data.Entity;
using SF.Core.Times;
using SF.Auth.Identities.Internals;
using SF.Auth.Identities.Models;

namespace SF.Auth.Identities.Entity
{
	public class EntityIdentityCredentialStorage<TModel> :
		IIdentityCredentialStorage
		where TModel : DataModels.IdentityCredential, new()
	{
		IDataSet<TModel> DataSet { get; }
		ITimeService TimeService { get; }
		public EntityIdentityCredentialStorage(
			IDataSet<TModel> DataSet,
			ITimeService TimeService
			)
		{
			this.DataSet = DataSet;
			this.TimeService = TimeService;
		}

		public async Task<IdentityCredential> FindOrBind(long Provider, string Credential, string UnionIdent, bool Confirmed, long UserId)
		{
			TModel exist;
			long? existUserId;
			if (UnionIdent != null)
			{
				var es = await DataSet.QueryAsync(i => i.Provider == Provider && i.UnionIdent == UnionIdent);
				exist = es.First(i => i.Credential == Credential);
				existUserId = exist?.IdentityId ?? es.FirstOrDefault()?.IdentityId;
			}
			else
			{
				exist = await DataSet.QuerySingleAsync(i => i.Provider == Provider && i.Credential == Credential);
				existUserId = exist?.IdentityId;
			}

			if (exist == null)
			{
				exist = DataSet.Add(new TModel
				{
					Provider = Provider,
					Credential = Credential,
					IdentityId = existUserId ?? UserId,
					UnionIdent = UnionIdent,
					CreatedTime = TimeService.Now,
					ConfirmedTime = Confirmed ? (DateTime?)TimeService.Now : null
				});
				await DataSet.Context.SaveChangesAsync();
			}
			return EntityMapper.Map<TModel, IdentityCredential>(exist);

		}

		public async Task<IdentityCredential> Find(long Provider, string Credential, string UnionIdent)
		{
			if (UnionIdent != null)
				return await DataSet.QuerySingleAsync(
					i => i.Provider == Provider && i.UnionIdent == UnionIdent && i.Credential == Credential,
					EntityMapper.Map<TModel, IdentityCredential>()
					);
			else
				return await DataSet.QuerySingleAsync(
					i => i.Provider == Provider && i.Credential == Credential,
					EntityMapper.Map<TModel, IdentityCredential>()
					);
		}

		public async Task Bind(long Provider, string Credential, string UnionIdent, bool Confirmed, long UserId)
		{
			DataSet.Add(new TModel
			{
				Provider = Provider,
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
			await DataSet.RemoveRangeAsync(i => i.Provider == Provider && i.Credential == Credential && i.IdentityId == UserId);
		}

		public async Task SetConfirmed(long Provider, string Credential, bool Confirmed)
		{
			await DataSet.Update(
				i =>i.Provider == Provider && i.Credential == Credential,
				e =>
				{
					e.ConfirmedTime = Confirmed ? (DateTime?)TimeService.Now : null;
				});
		}

		public async Task<IdentityCredential[]> GetIdents(long Provider, long UserId)
		{
			return await DataSet.QueryAsync(
				i => i.Provider == Provider && i.IdentityId == UserId,
				EntityMapper.Map<TModel, IdentityCredential>()
				);
		}
	}
	
}
