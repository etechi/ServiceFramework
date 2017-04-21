using SF.Data.Storage;
using SF.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using SF.Clients;
using SF.Data.Entity;
using SF.Core.Times;
using SF.Auth.Identity.Internals;
using SF.Auth.Identity.Models;

namespace SF.Auth.Identity.Entity
{
	public class EntityIdentBindStorage<TModel> :
		IIdentBindStorage
		where TModel : DataModels.IdentBind, new()
	{
		IDataSet<TModel> DataSet { get; }
		ITimeService TimeService { get; }
		public EntityIdentBindStorage(
			IDataSet<TModel> DataSet,
			ITimeService TimeService
			)
		{
			this.DataSet = DataSet;
			this.TimeService = TimeService;
		}

		public async Task<IdentBind> FindOrBind(int ScopeId, string Provider, string Ident, string UnionIdent, bool Confirmed, long UserId)
		{
			TModel exist;
			long? existUserId;
			if (UnionIdent != null)
			{
				var es = await DataSet.QueryAsync(i => i.ScopeId==ScopeId && i.Provider == Provider && i.UnionIdent == UnionIdent);
				exist = es.First(i => i.IdentValue == Ident);
				existUserId = exist?.IdentId ?? es.FirstOrDefault()?.IdentId;
			}
			else
			{
				exist = await DataSet.QuerySingleAsync(i => i.Provider == Provider && i.IdentValue == Ident);
				existUserId = exist?.IdentId;
			}

			if (exist == null)
			{
				exist = DataSet.Add(new TModel
				{
					Provider = Provider,
					IdentValue = Ident,
					IdentId = existUserId ?? UserId,
					UnionIdent = UnionIdent,
					CreatedTime = TimeService.Now,
					ConfirmedTime = Confirmed ? (DateTime?)TimeService.Now : null
				});
				await DataSet.Context.SaveChangesAsync();
			}
			return EntityMapper.Map<TModel, IdentBind>(exist);

		}

		public async Task<IdentBind> Find(int ScopeId, string Provider, string Ident, string UnionIdent)
		{
			if (UnionIdent != null)
				return await DataSet.QuerySingleAsync(
					i =>i.ScopeId== ScopeId && i.Provider == Provider && i.UnionIdent == UnionIdent && i.IdentValue == Ident,
					EntityMapper.Map<TModel, IdentBind>()
					);
			else
				return await DataSet.QuerySingleAsync(
					i =>i.ScopeId==ScopeId && i.Provider == Provider && i.IdentValue == Ident,
					EntityMapper.Map<TModel, IdentBind>()
					);
		}

		public async Task Bind(int ScopeId,string Provider, string Ident, string UnionIdent, bool Confirmed, long UserId)
		{
			DataSet.Add(new TModel
			{
				ScopeId=ScopeId,
				Provider = Provider,
				IdentValue = Ident,
				IdentId = UserId,
				UnionIdent = UnionIdent,
				CreatedTime = TimeService.Now,
				ConfirmedTime = Confirmed ? (DateTime?)TimeService.Now : null
			});
			await DataSet.Context.SaveChangesAsync();
		}

		public async Task Unbind(int ScopeId,string Provider, string Ident, long UserId)
		{
			await DataSet.RemoveRangeAsync(i => i.ScopeId==ScopeId && i.Provider == Provider && i.IdentValue == Ident && i.IdentId == UserId);
		}

		public async Task SetConfirmed(int ScopeId,string Provider, string Ident, bool Confirmed)
		{
			await DataSet.Update(
				i =>i.ScopeId==ScopeId && i.Provider == Provider && i.IdentValue == Ident,
				e =>
				{
					e.ConfirmedTime = Confirmed ? (DateTime?)TimeService.Now : null;
				});
		}

		public async Task<IdentBind[]> GetIdents(string Provider, long UserId)
		{
			return await DataSet.QueryAsync(
				i => i.Provider == Provider && i.IdentId == UserId,
				EntityMapper.Map<TModel, IdentBind>()
				);
		}
	}
	
}
