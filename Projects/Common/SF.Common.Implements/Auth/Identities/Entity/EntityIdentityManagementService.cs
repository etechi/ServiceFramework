using SF.Data.Storage;
using SF.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using SF.Auth.Identities.Models;
using SF.Auth.Identities.Internals;
using SF.Core.Caching;
using SF.Data.Entity;
using SF.Auth.Identities.DataModels;
using SF.Core.Times;

namespace SF.Auth.Identities.Entity
{
	public class EntityIdentityManagementService :
		QuerableEntitySource<long, Models.IdentityInternal, IdentityQueryArgument, DataModels.Identity>,
		IIdentityManagementService,
		IIdentStorage
	{
		public Lazy<ITimeService> TimeService { get; }
		public EntityIdentityManagementService(
			IDataSet<DataModels.Identity> DataSet,
			Lazy<ITimeService> TimeService
			) : base(DataSet)
		{
			this.TimeService = TimeService;
		}

		protected override PagingQueryBuilder<DataModels.Identity> PagingQueryBuilder =>
			PagingQueryBuilder<DataModels.Identity>.Simple("time", b => b.CreatedTime, true);

		protected override IContextQueryable<DataModels.Identity> OnBuildQuery(IContextQueryable<DataModels.Identity> Query, IdentityQueryArgument Arg, Paging paging)
		{
			return Query.Filter(Arg.Id, r => r.Id)
				.FilterContains(Arg.Ident,r=>r.SignupIdentValue);
				
		}

		async Task<long> IIdentStorage.Create(IdentCreateArgument Arg)
		{
			Ensure.Positive(Arg.Id, "未指定Id");
			Ensure.HasContent(Arg.IdentProvider, "未制定标识提供者");
			Ensure.HasContent(Arg.IdentValue, "未指定标识");
			Ensure.NotNull(Arg.SecurityStamp, "未制定安全戳");

			var time = TimeService.Value.Now;
			DataSet.Add(new DataModels.Identity
			{
				AppId =Arg.AppId,
				ScopeId=Arg.ScopeId,
				CreatedTime= time,
				Id=Arg.Id,
				ObjectState=LogicObjectState.Enabled,
				PasswordHash=Arg.PasswordHash,
				SecurityStamp=Arg.SecurityStamp.Base64(),
				SignupIdentProvider=Arg.IdentProvider,
				SignupIdentValue=Arg.IdentValue,
				UpdatedTime= time,
				IdentBinds=new[]
				{
					new DataModels.IdentityCredential
					{
						AppId=Arg.AppId,
						CreatedTime=time,
						ConfirmedTime=time,
						IdentityId=Arg.Id,
						Credential=Arg.IdentValue,
						Provider=Arg.IdentProvider,
						ScopeId=Arg.ScopeId,
					}
				}
			});

			await DataSet.Context.SaveChangesAsync();
			return Arg.Id;
		}


		async Task<IdentityData> IIdentStorage.Load(long Id)
		{
			var re=await DataSet.AsQueryable().Where(i => i.Id == Id).Select(i => new 
			{
				stamp=i.SecurityStamp,
				data=new IdentityData
				{
					Id=i.Id,
					Icon=i.Icon,
					Name=i.Name,
					IsEnabled=i.ObjectState==LogicObjectState.Enabled,
					PasswordHash=i.PasswordHash,
				}
			}).SingleOrDefaultAsync();
			if (re == null) return null;
			re.data.SecurityStamp = re.stamp.Base64();
			return re.data;
		}

		async Task IIdentStorage.UpdateDescription(Models.Identity Identity)
		{
			await DataSet.Update(Identity.Id, r =>
			{
				r.Name = Identity.Name;
				r.Icon = Identity.Icon;
			});
		}

		async Task IIdentStorage.UpdateSecurity(long Id, string PasswordHash, byte[] SecurityStamp)
		{
			await DataSet.Update(Id, r =>
			{
				r.PasswordHash = PasswordHash;
				r.SecurityStamp = SecurityStamp.Base64();
			});
		}
	}

}
