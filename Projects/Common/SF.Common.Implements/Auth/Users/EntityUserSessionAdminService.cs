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
	public class EntityUserSessionAdminService<TDataModel,TUserSessionInternal,TUserSessionQueryArgument> :
		SF.Data.Entity.EntitySource<long,TUserSessionInternal, TDataModel>,
		IUserSessionStorage
		where TDataModel:DataModels.UserSession,new()
		where TUserSessionInternal:UserSessionInternal,new()
		where TUserSessionQueryArgument:UserSessionQueryArgument
	{
		
		public ITimeService TimeService { get; }

		public EntityUserSessionAdminService(
			IDataSet<TDataModel> DataSet,
			ITimeService TimeService
			) :base(DataSet)
		{
			this.TimeService = TimeService;
		}
		protected virtual Task OnInitSession(TDataModel Model,UserType UserType,long UserId,AccessInfo AccessInfo)
		{
			Model.ClientAddress = AccessInfo.ClientAddress;
			Model.ClientAgent = AccessInfo.ClientAgent;
			Model.ClientType = AccessInfo.DeviceType;
			Model.CreatedTime = TimeService.Now;
			Model.LastActiveTime = Model.CreatedTime;
			Model.UserId = UserId;
			Model.UserType = UserType;
			return Task.CompletedTask;
		}
		async Task<long> IUserSessionStorage.Create(UserType UserType, long UserId, AccessInfo AccessInfo)
		{
			var model = new TDataModel();
			await OnInitSession(model, UserType, UserId, AccessInfo);
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
	}

	public class EntityUserSessionAdminService :
		EntityUserSessionAdminService<DataModels.UserSession, UserSessionInternal, UserSessionQueryArgument>
	{
		public EntityUserSessionAdminService(IDataSet<DataModels.UserSession> DataSet, ITimeService TimeService) : base(DataSet, TimeService)
		{
		}
	}
}
