using System;
using System.Linq;
using System.Threading.Tasks;
using SF.Auth;
using SF.Data;
using SF.Data.Entity;
using SF.Data.Storage;
using SF.Metadata;
using SF.Core.ServiceManagement.Models;
using SF.Core.ServiceManagement.Internals;
using SF.Core.DI;
using SF.Core.Logging;
using System.Collections.Generic;
using SF.Core.Times;
using System.Reflection;

namespace SF.Core.ServiceManagement.Management
{
	[Comment("Ĭ�Ϸ���ʵ������")]
	public class ServiceInstanceManager :
		EntityManager<
			long, 
			Models.ServiceInstanceInternal, 
			ServiceInstanceQueryArgument, 
			Models.ServiceInstanceEditable, 
			DataModels.ServiceInstance
			>,
		IServiceInstanceManager
		// IServiceConfigLoader,
		//IServiceInstanceLister
	{
		IDataEntityResolver EntityResolver { get;}
		Lazy<IServiceProvider> ServiceProvider { get; }
		public IServiceInstanceConfigChangedNotifier ConfigChangedNotifier { get; }
		ILogger<ServiceInstanceManager> Logger { get; }
		Lazy<IIdentGenerator> IdentGenerator { get; }
		ITimeService TimeService { get; }
		IServiceMetadata Metadata { get; }
		public ServiceInstanceManager(
			IDataSet<DataModels.ServiceInstance> DataSet, 
			IDataEntityResolver EntityResolver,
			IServiceInstanceConfigChangedNotifier ConfigChangedNotifier,
			Lazy<IServiceProvider> ServiceProvider,
			ILogger<ServiceInstanceManager> Logger,
			Lazy<IIdentGenerator> IdentGenerator,
			ITimeService TimeService,
			IServiceMetadata Metadata
			) : 
			base(DataSet)
		{
			this.ServiceProvider = ServiceProvider;
			this.EntityResolver = EntityResolver;
			this.ConfigChangedNotifier = ConfigChangedNotifier;
			this.Logger = Logger;
			this.IdentGenerator = IdentGenerator;
			this.TimeService = TimeService;
			this.Metadata = Metadata;
		}

		protected override PagingQueryBuilder<DataModels.ServiceInstance> PagingQueryBuilder =>
			new PagingQueryBuilder<DataModels.ServiceInstance>(
				"name",
				b => b.Add("name", i => i.Name));


		protected override async Task<Models.ServiceInstanceEditable> OnMapModelToEditable(IContextQueryable<DataModels.ServiceInstance> Query)
		{
			var re= await Query.Select(i => new Models.ServiceInstanceEditable
			{
				Id = i.Id,
				//ImplementId = i.ImplementId,
				ObjectState = i.ObjectState,
				Priority = i.Priority,
				ServiceIdent=i.ServiceIdent,
				Name = i.Name,
				Title = i.Title,
				CreatedTime = i.CreatedTime,
				ParentId=i.ParentId,
				ParentName=i.ParentId.HasValue?i.Parent.Name:null,
				DisplayData=new Data.Models.UIDisplayData
				{
					Description=i.Description,
					Icon=i.Icon,
					Image=i.Image,
					Remarks=i.Remarks,
					SubTitle=i.SubTitle
				},
				UpdatedTime = i.UpdatedTime,
				ServiceType = i.ServiceType,
				ImplementType = i.ImplementType,
				
				Setting= i.Setting
			}).SingleAsync();

			var svcDef = Metadata.ServicesByTypeName.Get(re.ServiceType);
			if (svcDef == null)
				throw new InvalidOperationException($"�Ҳ�������ķ���{re.ServiceType}");
			re.ServiceName = svcDef.ServiceType.Comment().Name;
			var (implType, _) = re.ImplementType.Split2('@');
			var svcImpl = svcDef.Implements.SingleOrDefault(i => i.ImplementType.FullName == implType);
			if (svcImpl == null)
				throw new InvalidOperationException($"�Ҳ�������ʵ��{re.ImplementType}");
			re.ImplementName = svcImpl.ImplementType.Comment().Name;


			re.SettingType = implType + "SettingType";

			Logger.Info("Load ServiceInstance for Edit: {0}",Json.Stringify(re));
			return re;
		}

		protected override IContextQueryable<Models.ServiceInstanceInternal> OnMapModelToPublic(IContextQueryable<DataModels.ServiceInstance> Query)
		{
			return Query.Select(i => new Models.ServiceInstanceInternal
			{
				Id = i.Id,
				ServiceType = i.ServiceType,
				//ImplementId = i.ImplementId,
				ObjectState = i.ObjectState,
				Priority = i.Priority,
				ServiceIdent=i.ServiceIdent,
				Name = i.Name,
				Title = i.Title,
				CreatedTime = i.CreatedTime,
				ServiceName = i.ServiceType,
				ImplementType = i.ImplementType,
				ImplementName = i.ImplementType,
				UpdatedTime = i.UpdatedTime,
				ParentId=i.ParentId,
				ParentName = i.ParentId.HasValue?i.Parent.Name:null
			});
		}

		protected override IContextQueryable<DataModels.ServiceInstance> OnBuildQuery(IContextQueryable<DataModels.ServiceInstance> Query, ServiceInstanceQueryArgument Arg, Paging paging)
		{
			if (Arg.Id.HasValue)
				return Query.Filter(Arg.Id, i => i.Id);
			else
				return Query
					.Filter(Arg.Name, i => i.Name)
					.Filter(Arg.ServiceType, i => i.ServiceType)
					.Filter(Arg.ServiceIdent,i=>i.ServiceIdent)
					.Filter(Arg.ImplementId, i => i.ImplementType)
					.Filter(Arg.IsDefaultService.HasValue? (Arg.IsDefaultService.Value?(int?)0:(int?)-1):null,i=>i.Priority)
				;
		}

		protected override async Task<Models.ServiceInstanceInternal[]> OnPreparePublics(Models.ServiceInstanceInternal[] Internals)
		{
			await EntityResolver.Fill(
				Internals,
				i => new[] { "ϵͳ����ʵ��-" + i.ImplementType, "ϵͳ������-" + i.ServiceType},
				(i,ns)=> {
					i.ImplementName=ns[0];
					i.ServiceName=ns[1];
				});
			return Internals;
		}
		class TestServiceInstanceDescriptor : IServiceInstanceDescriptor
		{
			public long InstanceId { get; set; }
			public long? ParentInstanceId { get; set; }
			public bool IsManaged => true;
			public IServiceDeclaration ServiceDeclaration { get; set; }
			public IServiceImplement ServiceImplement { get; set; }
		}
		void TestConfig(long Id, long? ParentId, string ImplementId, string CreateArguments)
		{
			var (implTypeName, svcTypeName) = ImplementId.Split2('@');
			var ServiceResolver = this.ServiceProvider.Value.NewResolver();

			var (decl, impl) = ServiceFactory.ResolveMetadata(
				ServiceResolver,
				Id,
				svcTypeName,
				implTypeName,
				null
				);

			var factory=ServiceFactory.Create(
				Id,
				ParentId,
				decl,
				impl,
				decl.ServiceType,
				null,
				ServiceResolver.Resolve<IServiceMetadata>(),
				CreateArguments
				);

			var svr = factory.Create(ServiceResolver);
			if (svr == null)
				throw new PublicArgumentException("��������ʧ��,���������Ƿ�����");
		}
		protected override async Task OnUpdateModel(ModifyContext ctx)
		{
			var e = ctx.Editable;
			var m = ctx.Model;
			UIEnsure.HasContent(e.ImplementType,"����ʵ��");
			UIEnsure.HasContent(e.ServiceType,"������");
			UIEnsure.HasContent(e.Name,"��������");
			UIEnsure.HasContent(e.Title, "�������");

			if (ctx.Action == ModifyAction.Create)
			{
				m.ServiceType = e.ServiceType;
				m.ImplementType = e.ImplementType;
				
			}
			else
			{
				UIEnsure.Equal(m.ServiceType, e.ServiceType,"�������Ͳ����޸�");
				UIEnsure.Equal(m.ImplementType, e.ImplementType, "����ʵ�����Ͳ����޸�");
			}
			m.ObjectState = e.ObjectState;
			m.Name = e.Name;
			m.Title = e.Title;

			m.Description = e.DisplayData?.Description;
			m.Icon = e.DisplayData?.Icon;
			m.Remarks = e.DisplayData?.Remarks;
			m.Image = e.DisplayData?.Image;
			m.UpdatedTime= TimeService.Now;
			m.Setting = e.Setting;

			m.SubTitle = e.DisplayData?.SubTitle;
			m.Icon = e.DisplayData?.Icon;
			m.Image = e.DisplayData?.Image;
			m.Description = e.DisplayData?.Description;
			m.Remarks = e.DisplayData?.Remarks;

			TestConfig(m.Id,m.ParentId, m.ImplementType, m.Setting);
			//var siis = DataSet.Context
			//		.Set<DataModels.ServiceInstanceInterface>();
			//var orgs = await siis.LoadListAsync(sii => sii.ServiceInstanceId == m.Id);
			//siis.Merge<DataModels.ServiceInstanceInterface,Models.ServiceInstanceInterface>(
			//	orgs,
			//	e?.Interfaces,
			//	(mi, ei) => mi.InterfaceType == ei.InterfaceType,
			//	ei => new DataModels.ServiceInstanceInterface
			//	{
			//		ServiceInstanceId=m.Id,
			//		InterfaceType=ei.InterfaceType,
			//		ImplementType=ei.ImplementType,
			//		Setting=ei.Setting
			//	},
			//	(mi, ei) => mi.Setting = ei.Setting
			//	);

			if(m.ParentId != e.ParentId)
			{
				var orgParentId = m.ParentId;
				m.ParentId = e.ParentId;
				ctx.AddPostAction(() =>
				{
					ConfigChangedNotifier.NotifyInternalServiceChanged(
						orgParentId,
						m.ServiceType
						);
					ConfigChangedNotifier.NotifyInternalServiceChanged(
						e.ParentId,
						m.ServiceType
						);
				});
			}
			

			if (await DataSet.ModifyPosition(
				m,
				PositionModifyAction.Insert,
				i => i.ParentId == m.ParentId && i.ServiceType==m.ServiceType,
				i => i.Id == m.Id,
				i => i.Priority,
				(i, p) => i.Priority = p
				) || m.ServiceIdent != e.ServiceIdent)
				ctx.AddPostAction(() => 
					ConfigChangedNotifier.NotifyInternalServiceChanged(
						m.ParentId,
						m.ServiceType
						)
					);
			m.ServiceIdent = e.ServiceIdent;

			ctx.AddPostAction(() =>
			{
				ConfigChangedNotifier.NotifyChanged( m.Id);
				Logger.Info("ServiceInstance Saved:{0}",Json.Stringify(m));
			});
		}
		protected override async Task OnNewModel(ModifyContext ctx)
		{
			//UIEnsure.NotNull(ctx.Model.Id, "δ���÷���ʵ��ID");
			ctx.Model.Id = await IdentGenerator.Value.GenerateAsync("ϵͳ����", 0);
			ctx.Model.CreatedTime = TimeService.Now;
			await base.OnNewModel(ctx);
		}
		protected override async Task OnRemoveModel(ModifyContext ctx)
		{
			//if (ctx.Model.IsDefaultService)
				//throw new PublicInvalidOperationException("����ɾ��Ĭ�Ϸ���");

			await base.OnRemoveModel(ctx);
		}

		//class Config : IServiceConfig
		//{
		//	public long Id { get; set; }

		//	public long? ParentId { get; set; }

		//	public string ServiceType { get; set; }

		//	public string ImplementType { get; set; }

		//	public string Setting { get; set; }
		//	public string Name { get; set; }
		//}
		//IServiceConfig IServiceConfigLoader.GetConfig(long Id)
		//{
		//	var re = DataSet.QuerySingleAsync(
		//		si => si.Id == Id,
		//		si => new Config
		//		{
					
		//			Id = Id,
		//			ServiceType = si.ServiceType,
		//			ImplementType = si.ImplementType,
		//			Setting= si.Setting,
		//			ParentId=si.ParentId
		//		}
		//		).Result;

		//	var svcDef = Metadata.ServicesByTypeName.Get(re.ServiceType);
		//	if (svcDef == null)
		//		throw new InvalidOperationException($"�Ҳ�������ķ���{re.ServiceType}");

		//	var svcImpl = svcDef.Implements.SingleOrDefault(i => i.ImplementType.FullName == re.ImplementType);
		//	if(svcImpl==null)
		//		throw new InvalidOperationException($"�Ҳ�������ʵ��{re.ImplementType}");

		//	return re;
		//}
		
		//ServiceReference[] IServiceInstanceLister.List(long? ScopeId,string ServiceType,int Limit)
		//{
		//	var re = DataSet.AsQueryable()
		//		.Where(si => si.ParentId == ScopeId && si.ServiceType == ServiceType)
		//		.OrderBy(si => si.Priority)
		//		.Select(si => new ServiceReference
		//		{
		//			Id = si.Id,
		//			Name = si.ServiceIdent
		//		})
		//		.Take(Limit)
		//		.ToArray();
		//	return re;
		//}
	}

}
