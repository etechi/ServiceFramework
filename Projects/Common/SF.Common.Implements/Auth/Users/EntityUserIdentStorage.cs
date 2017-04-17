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

namespace SF.Auth.Identity
{
	public class EntityUserIdentStorage<TModel> :
		IIdentBindStorage
		where TModel : DataModels.UserIdent, new()
	{
		IDataSet<TModel> DataSet { get; }
		ITimeService TimeService { get; }
		public EntityUserIdentStorage(
			IDataSet<TModel> DataSet,
			ITimeService TimeService
			)
		{
			this.DataSet = DataSet;
			this.TimeService = TimeService;
		}

		public async Task<IdentBind> FindOrBind(string Provider, string Ident, string UnionIdent, bool Confirmed, long UserId)
		{
			TModel exist;
			long? existUserId;
			if (UnionIdent != null)
			{
				var es = await DataSet.QueryAsync(i => i.Provider == Provider && i.UnionIdent == UnionIdent);
				exist = es.First(i => i.Ident == Ident);
				existUserId = exist?.UserId ?? es.FirstOrDefault()?.UserId;
			}
			else
			{
				exist = await DataSet.QuerySingleAsync(i => i.Provider == Provider && i.Ident == Ident);
				existUserId = exist?.UserId;
			}

			if (exist == null)
			{
				exist = DataSet.Add(new TModel
				{
					Provider = Provider,
					Ident = Ident,
					UserId = existUserId ?? UserId,
					UnionIdent = UnionIdent,
					BindTime = TimeService.Now,
					ConfirmedTime = Confirmed ? (DateTime?)TimeService.Now : null
				});
				await DataSet.Context.SaveChangesAsync();
			}
			return EntityMapper.Map<TModel, IdentBind>(exist);

		}

		public async Task<IdentBind> Find(string Provider, string Ident, string UnionIdent)
		{
			if (UnionIdent != null)
				return await DataSet.QuerySingleAsync(
					i => i.Provider == Provider && i.UnionIdent == UnionIdent && i.Ident == Ident,
					EntityMapper.Map<TModel, IdentBind>()
					);
			else
				return await DataSet.QuerySingleAsync(
					i => i.Provider == Provider && i.Ident == Ident,
					EntityMapper.Map<TModel, IdentBind>()
					);
		}

		public async Task Bind(string Provider, string Ident, string UnionIdent, bool Confirmed, long UserId)
		{
			DataSet.Add(new TModel
			{
				Provider = Provider,
				Ident = Ident,
				UserId = UserId,
				UnionIdent = UnionIdent,
				BindTime = TimeService.Now,
				ConfirmedTime = Confirmed ? (DateTime?)TimeService.Now : null
			});
			await DataSet.Context.SaveChangesAsync();
		}

		public async Task Unbind(string Provider, string Ident, long UserId)
		{
			await DataSet.RemoveRangeAsync(i => i.Provider == Provider && i.Ident == Ident && i.UserId == UserId);
		}

		public async Task SetConfirmed(string Provider, string Ident, bool Confirmed)
		{
			await DataSet.Update(
				i => i.Provider == Provider && i.Ident == Ident,
				e =>
				{
					e.ConfirmedTime = Confirmed ? (DateTime?)TimeService.Now : null;
				});
		}

		public async Task<IdentBind[]> GetIdents(string Provider, long UserId)
		{
			return await DataSet.QueryAsync(
				i => i.Provider == Provider && i.UserId == UserId,
				EntityMapper.Map<TModel, IdentBind>()
				);
		}
	}

	public class EntityUserIdentStorage :
		EntityUserIdentStorage<DataModels.UserIdent>
	{
		public EntityUserIdentStorage(IDataSet<DataModels.UserIdent> DataSet, ITimeService TimeService) : base(DataSet, TimeService)
		{
		}
	}
}
