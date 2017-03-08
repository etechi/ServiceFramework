using SF.Data.Storage;
using SF.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using SF.Clients;
using SF.Data.Entity;
using SF.Core.Times;
using SF.Auth.Users.DataModels;

namespace SF.Auth.Users
{
	public class EntityUserIdentStorage<TModel> :
		IUserIdentStorage
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

		public async Task<UserIdent> FindOrBind(string Provider, string Ident, string UnionIdent, bool Confirmed, long UserId)
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
			return EntityMapper.Map<TModel, UserIdent>(exist);

		}

		public async Task<UserIdent> Find(string Provider, string Ident, string UnionIdent)
		{
			if (UnionIdent != null)
				return await DataSet.QuerySingleAsync(
					i => i.Provider == Provider && i.UnionIdent == UnionIdent && i.Ident == Ident,
					EntityMapper.Map<TModel, UserIdent>()
					);
			else
				return await DataSet.QuerySingleAsync(
					i => i.Provider == Provider && i.Ident == Ident,
					EntityMapper.Map<TModel, UserIdent>()
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

		public async Task<UserIdent[]> GetIdents(string Provider, long UserId)
		{
			return await DataSet.QueryAsync(
				i => i.Provider == Provider && i.UserId == UserId,
				EntityMapper.Map<TModel, UserIdent>()
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
