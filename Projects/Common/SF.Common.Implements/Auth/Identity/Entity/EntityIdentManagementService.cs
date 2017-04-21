using SF.Data.Storage;
using SF.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using SF.Auth.Identity.Models;
using SF.Auth.Identity.Internals;
using SF.Core.Caching;
using SF.Data.Entity;
using SF.Auth.Identity.DataModels;
using SF.Core.Times;

namespace SF.Auth.Identity.Entity
{
	public class EntityIdentManagementService :
		QuerableEntitySource<long, Models.IdentInternal, IdentQueryArgument, DataModels.Ident>,
		IIdentManagementService,
		IIdentStorage
	{
		public Lazy<ITimeService> TimeService { get; }
		public EntityIdentManagementService(
			IDataSet<Ident> DataSet,
			Lazy<ITimeService> TimeService
			) : base(DataSet)
		{
			this.TimeService = TimeService;
		}

		protected override PagingQueryBuilder<Ident> PagingQueryBuilder =>
			PagingQueryBuilder<Ident>.Simple("time", b => b.CreatedTime, true);

		protected override IContextQueryable<Ident> OnBuildQuery(IContextQueryable<Ident> Query, IdentQueryArgument Arg, Paging paging)
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
			DataSet.Add(new DataModels.Ident
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
					new DataModels.IdentBind
					{
						AppId=Arg.AppId,
						CreatedTime=time,
						ConfirmedTime=time,
						IdentId=Arg.Id,
						IdentValue=Arg.IdentValue,
						Provider=Arg.IdentProvider,
						ScopeId=Arg.ScopeId,
					}
				}
			});

			await DataSet.Context.SaveChangesAsync();
			return Arg.Id;
		}

		async Task<string> IIdentStorage.GetPasswordHash(long Id)
		{
			return await DataSet.FieldAsync(r => r.Id == Id, r => r.PasswordHash);
		}
		async Task<bool> IIdentStorage.IsEnabled(long Id)
		{
			return LogicObjectState.Enabled== await DataSet.FieldAsync(r => r.Id == Id, r => r.ObjectState);
		}

		async Task<byte[]> IIdentStorage.GetSecurityStamp(long Id)
		{
			var re=await DataSet.FieldAsync(r => r.Id == Id, r => r.SecurityStamp);
			return re?.Base64();
		}

		async Task IIdentStorage.SetPasswordHash(long Id, string PasswordHash, byte[] SecurityStamp)
		{
			await DataSet.Update(Id, r =>
			{
				r.PasswordHash = PasswordHash;
				r.SecurityStamp = SecurityStamp.Base64();
			});
		}
	}

}
