using SF.Auth.Identities.Internals;
using SF.Auth.Identities.Models;
using SF.Core.ServiceManagement;
using SF.Core.Times;
using SF.Data;
using SF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Auth.Identities.Entity
{
	public class EntityIdentityManagementService<TIdentity,TIdentityCredential> :
		//QuerableEntitySource<long, Models.IdentityInternal, IdentityQueryArgument, TIdentity>,
		EntityManager<
			long,
			Models.IdentityInternal,
			IdentityQueryArgument,
			Models.IdentityEditable,
			TIdentity
			>,
		IIdentityManagementService,
		IIdentStorage
		where TIdentity:DataModels.Identity<TIdentity,TIdentityCredential>,new()
		where TIdentityCredential : DataModels.IdentityCredential<TIdentity, TIdentityCredential>,new()
	{
		public Lazy<ITimeService> TimeService { get; }
		public IServiceInstanceDescriptor ServiceInstanceDescroptor { get; }
		public EntityIdentityManagementService(
			IDataSet<TIdentity> DataSet,
			Lazy<ITimeService> TimeService,
			IServiceInstanceDescriptor ServiceInstanceDescroptor
			) : base(DataSet)
		{
			this.TimeService = TimeService;
			this.ServiceInstanceDescroptor = ServiceInstanceDescroptor;
		}

		protected override PagingQueryBuilder<TIdentity> PagingQueryBuilder =>
			PagingQueryBuilder<TIdentity>.Simple("time", b => b.CreatedTime, true);

		protected override IContextQueryable<TIdentity> OnBuildQuery(IContextQueryable<TIdentity> Query, IdentityQueryArgument Arg, Paging paging)
		{
			var sid = ServiceInstanceDescroptor.InstanceId;
			return Query
				.Where(r=>r.ScopeId==sid)
				.Filter(Arg.Id, r => r.Id)
				.FilterContains(Arg.Ident,r=>r.SignupIdentValue);
				
		}

		async Task<long> IIdentStorage.Create(IdentityCreateArgument Arg)
		{
			await CreateAsync(new IdentityEditable
			{
				Id = Arg.Identity.Id,
				Name = Arg.Identity.Name,
				Icon = Arg.Identity.Icon,

				CreateCredential = Arg.CredentialValue,
				CreateCredentialProviderId = Arg.CredentialProvider,

				ObjectState = LogicEntityState.Enabled,

				PasswordHash = Arg.PasswordHash,
				SecurityStamp = Arg.SecurityStamp.Base64(),
				Entity = Arg.Identity.Entity,
				Credentials=new List<IdentityCredential>
				{
					new IdentityCredential
					{
						Credential=Arg.CredentialValue,
						ProviderId=Arg.CredentialProvider,
					}
				}
			});
			return Arg.Identity.Id;

			//Ensure.NotNull(Arg.Identity, "身份标识");
			//Ensure.HasContent(Arg.Identity.Name, "身份标识名称");
			//Ensure.Positive(Arg.Identity.Id, "身份标识ID");
			//Ensure.HasContent(Arg.Identity.Entity, "身份类型");
			//Ensure.Positive(Arg.CredentialProvider, "未制定标识提供者");
			//Ensure.HasContent(Arg.CredentialValue, "未指定标识");
			//Ensure.NotNull(Arg.SecurityStamp, "未制定安全戳");
			

			//var time = TimeService.Value.Now;
			//DataSet.Add(new TIdentity
			//{
			//	//AppId =Arg.AppId,
			//	//ScopeId=Arg.ScopeId,
			//	CreatedTime= time,
			//	Id=Arg.Identity.Id,
			//	Name=Arg.Identity.Name,
			//	Icon=Arg.Identity.Icon,
			//	ObjectState=LogicObjectState.Enabled,
			//	PasswordHash=Arg.PasswordHash,
			//	SecurityStamp=Arg.SecurityStamp.Base64(),
			//	SignupIdentProviderId=Arg.CredentialProvider,
			//	SignupIdentValue=Arg.CredentialValue,
			//	UpdatedTime= time,
			//	Credentials=new[]
			//	{
			//		new TIdentityCredential
			//		{
			//			//AppId=Arg.AppId,
			//			//ScopeId=Arg.ScopeId,
			//			CreatedTime =time,
			//			ConfirmedTime=time,
			//			IdentityId=Arg.Identity.Id,
			//			Credential=Arg.CredentialValue,
			//			ProviderId=Arg.CredentialProvider,
						
			//		}
			//	}
			//});
		
			//await DataSet.Context.SaveChangesAsync();
			//return Arg.Identity.Id;
		}
		protected override Task OnNewModel(ModifyContext ctx)
		{
			var e = ctx.Editable;
			var m = ctx.Model;
			m.Id = e.Id;
			m.SignupIdentProviderId = e.CreateCredentialProviderId;
			m.SignupIdentValue = e.CreateCredential;
			m.CreatedTime = TimeService.Value.Now;
			m.ScopeId = ServiceInstanceDescroptor.InstanceId;
			return base.OnNewModel(ctx);
		}
		
		protected override async Task OnUpdateModel(ModifyContext ctx)
		{
			var e = ctx.Editable;
			var m = ctx.Model;
			m.Name = e.Name;
			m.Icon = e.Icon;

			m.ObjectState = e.ObjectState;
			var time = TimeService.Value.Now;
			m.UpdatedTime = time;
			if (e.Entity != null)
				m.Entity = e.Entity;
			if (e.PasswordHash!=null)
				m.PasswordHash = e.PasswordHash;
			if(e.SecurityStamp!=null)
				m.SecurityStamp = e.SecurityStamp;
			if (e.Credentials != null)
			{
				var ics = DataSet.Context.Set<TIdentityCredential>();
				var oitems = await ics.LoadListAsync(ic => ic.IdentityId == m.Id);
				ics.Merge(
					oitems,
					e.Credentials,
					(mi, ei) => mi.ProviderId == ei.ProviderId && mi.Credential==ei.Credential,
					ei => new TIdentityCredential
					{
						ScopeId = m.ScopeId,
						IdentityId = m.Id,
						UnionIdent = ei.UnionIdent,
						Credential = ei.Credential,
						ProviderId = ei.ProviderId,
						CreatedTime = time,
					},
					(mi, ei) =>
					{
						mi.UnionIdent = ei.UnionIdent;
					}
					);

			}
		}
		protected override async Task OnRemoveModel(ModifyContext ctx)
		{
			var ics = DataSet.Context.Set<TIdentityCredential>();
			await ics.RemoveRangeAsync(ic => ic.IdentityId == ctx.Model.Id);
			await base.OnRemoveModel(ctx);
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
					Entity=i.Entity,
					IsEnabled=i.ObjectState==LogicEntityState.Enabled,
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
