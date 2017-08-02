using SF.Data.Storage;
using SF.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using SF.Auth.Identities.Models;
using SF.Auth.Identities.Internals;
using SF.Core.Caching;
using SF.Data.Entity;
using SF.Core.Times;

namespace SF.Auth.Identities.Entity
{
	public class EntityIdentityManagementService<TIdentity,TIdentityCredential> :
		QuerableEntitySource<long, Models.IdentityInternal, IdentityQueryArgument, TIdentity>,
		IIdentityManagementService,
		IIdentStorage
		where TIdentity:DataModels.Identity<TIdentity,TIdentityCredential>,new()
		where TIdentityCredential : DataModels.IdentityCredential<TIdentity, TIdentityCredential>,new()
	{
		public Lazy<ITimeService> TimeService { get; }
		public EntityIdentityManagementService(
			IDataSet<TIdentity> DataSet,
			Lazy<ITimeService> TimeService
			) : base(DataSet)
		{
			this.TimeService = TimeService;
		}

		protected override PagingQueryBuilder<TIdentity> PagingQueryBuilder =>
			PagingQueryBuilder<TIdentity>.Simple("time", b => b.CreatedTime, true);

		protected override IContextQueryable<TIdentity> OnBuildQuery(IContextQueryable<TIdentity> Query, IdentityQueryArgument Arg, Paging paging)
		{
			return Query.Filter(Arg.Id, r => r.Id)
				.FilterContains(Arg.Ident,r=>r.SignupIdentValue);
				
		}

		async Task<long> IIdentStorage.Create(IdentityCreateArgument Arg)
		{
			Ensure.NotNull(Arg.Identity, "身份标识");
			Ensure.HasContent(Arg.Identity.Name, "身份标识名称");
			Ensure.Positive(Arg.Identity.Id, "身份标识ID");
			Ensure.HasContent(Arg.Identity.Entity, "身份类型");
			Ensure.Positive(Arg.IdentProvider, "未制定标识提供者");
			Ensure.HasContent(Arg.CredentialValue, "未指定标识");
			Ensure.NotNull(Arg.SecurityStamp, "未制定安全戳");
			

			var time = TimeService.Value.Now;
			DataSet.Add(new TIdentity
			{
				//AppId =Arg.AppId,
				//ScopeId=Arg.ScopeId,
				CreatedTime= time,
				Id=Arg.Identity.Id,
				Name=Arg.Identity.Name,
				Icon=Arg.Identity.Icon,
				ObjectState=LogicObjectState.Enabled,
				PasswordHash=Arg.PasswordHash,
				SecurityStamp=Arg.SecurityStamp.Base64(),
				SignupIdentProvider=Arg.IdentProvider,
				SignupIdentValue=Arg.CredentialValue,
				UpdatedTime= time,
				Credentials=new[]
				{
					new TIdentityCredential
					{
						//AppId=Arg.AppId,
						//ScopeId=Arg.ScopeId,
						CreatedTime =time,
						ConfirmedTime=time,
						IdentityId=Arg.Identity.Id,
						Credential=Arg.CredentialValue,
						ProviderId=Arg.IdentProvider,
						
					}
				}
			});

			await DataSet.Context.SaveChangesAsync();
			return Arg.Identity.Id;
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
