using SF.Data.Storage;
using SF.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using SF.Clients;
using SF.Data.Entity;
using SF.Core.Times;
using SF.Auth.Passport.DataModels;
using SF.Auth.Passport.Models;
using SF.Auth.Passport.Internals;

namespace SF.Auth.Passport
{
	public class EntityUserSessionAdminService<TDataModel> :
		SF.Data.Entity.QuerableEntitySource<long, UserSessionInternal, UserSessionQueryArgument, TDataModel>,
		IUserSessionAdminService,
		IUserSessionStorage
		where TDataModel:DataModels.UserSession,new()
	{
		protected override PagingQueryBuilder<TDataModel> PagingQueryBuilder => new PagingQueryBuilder<TDataModel>(
				"time",
				b => b.Add("time", i => i.CreatedTime, true));

		public ITimeService TimeService { get; }

		public EntityUserSessionAdminService(
			IDataSet<TDataModel> DataSet,
			ITimeService TimeService
			) :base(DataSet)
		{
			this.TimeService = TimeService;
		}
		protected virtual Task OnInitSession(TDataModel Model,long UserId, DateTime? Expires, IAccessSource AccessInfo)
		{
			Model.ClientAddress = AccessInfo.ClientAddress;
			Model.ClientAgent = AccessInfo.ClientAgent;
			Model.ClientType = AccessInfo.DeviceType;
			Model.CreatedTime = TimeService.Now;
			Model.LastActiveTime = Model.CreatedTime;
			Model.Expires = Expires;
			Model.UserId = UserId;
			return Task.CompletedTask;
		}
		async Task<long> IUserSessionStorage.Create( long UserId,DateTime? Expires, IAccessSource AccessInfo)
		{
			var model = new TDataModel();
			await OnInitSession(model, UserId, Expires, AccessInfo);
			DataSet.Add(model);
			await DataSet.Context.SaveChangesAsync();
			return model.Id;
		}

		async Task IUserSessionStorage.Update(long Id)
		{
			await DataSet.Update(
				Id,
				m => m.LastActiveTime = TimeService.Now
				);
		}

		protected override IContextQueryable<TDataModel> OnBuildQuery(IContextQueryable<TDataModel> Query, UserSessionQueryArgument Arg, Paging paging)
		{
			return Query;
			//return Query.Filter(Arg.NickName, e => e.NickName);
		}

		protected override Task<UserSessionInternal[]> OnPreparePublics(UserSessionInternal[] Internals)
		{
			throw new NotImplementedException();
		}
	}
}
