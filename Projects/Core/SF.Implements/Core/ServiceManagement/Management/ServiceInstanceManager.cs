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
		IServiceInstanceManager,
		 IServiceConfigLoader, 
		IDefaultServiceLocator
	{
		IDataEntityResolver EntityResolver { get;}
		IServiceProvider ServiceProvider { get; }
		public IServiceInstanceConfigChangedNotifier ConfigChangedNotifier { get; }
		ILogger<ServiceInstanceManager> Logger { get; }
		Lazy<IIdentGenerator> IdentGenerator { get; }
		ITimeService TimeService { get; }
		IServiceMetadata Metadata { get; }
		public ServiceInstanceManager(
			IDataSet<DataModels.ServiceInstance> DataSet, 
			IDataEntityResolver EntityResolver,
			IServiceInstanceConfigChangedNotifier ConfigChangedNotifier,
			IServiceProvider ServiceProvider,
			ILogger<ServiceInstanceManager> Logger,
			IServiceMetadata ServiceMetadata,
			Lazy<IIdentGenerator> IdentGenerator,
			TimeService TimeService,
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
				IsDefaultService = i.IsDefaultService,
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
			var svcImpl = svcDef.Implements.SingleOrDefault(i => i.ImplementType.FullName == re.ImplementType);
			if (svcImpl == null)
				throw new InvalidOperationException($"�Ҳ�������ʵ��{re.ImplementType}");
			re.ImplementName = svcImpl.ImplementType.Comment().Name;


			re.SettingType = re.ImplementType + "SettingType";

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
				IsDefaultService = i.IsDefaultService,
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
				return Query.Filter(Arg.Id.Value, i => i.Id);
			else
				return Query
					.Filter(Arg.Name, i => i.Name)
					.Filter(Arg.DeclarationId, i => i.ServiceType)
					//.Filter(Arg.ImplementId, i => i.ImplementId)
					.Filter(Arg.IsDefaultService,i=>i.IsDefaultService)
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
		//void TestConfig(string Id,string ImplementId,string CreateArguments)
		//{
		//	var ServiceScope = ServiceProvider.Resolve<IManagedServiceScope>();
		//	var ImplementTypeResolver = ServiceProvider.Resolve<IServiceImplementTypeResolver>();
		//	var ServiceMetadata = ServiceProvider.Resolve<IServiceMetadata>();
		//	var ImplementType = ImplementTypeResolver.Resolve(ImplementId.Split2('@').Item1);
		//	var ci = Runtime.ServiceFactoryBuilder.FindBestConstructorInfo(ImplementType);
		//	var tmpl = Runtime.ServiceCreateParameterTemplate.Load(
		//			ci,
		//			Id,
		//			CreateArguments,
		//			ServiceMetadata
		//			);

		//	var factory = Runtime.ServiceFactoryBuilder.Build(
		//		ServiceMetadata,
		//		ImplementType,
		//		ci
		//		);
		//	var svr=factory(ServiceProvider, ServiceScope, tmpl);
		//	if (svr == null)
		//		throw new PublicArgumentException("���ô���,��������ʧ��");
		//}
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
				m.ParentId = e.ParentId;
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
			//TestConfig(m.Id, m.ImplementId, m.CreateArguments);
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

			if (await DataSet.TrySetDefaultItem(
				m,
				e.IsDefaultService,
				i => i.IsDefaultService,
				(i, d) => i.IsDefaultService = d,
				i => i.ServiceType == m.ServiceType,
				i => i.Id == m.Id,
				i=>i.IsDefaultService
				))
				ctx.AddPostAction(() => ConfigChangedNotifier.NotifyDefaultChanged(m.ParentId,m.ServiceType));

			ctx.AddPostAction(() =>
			{
				ConfigChangedNotifier.NotifyChanged( m.Id);
				Logger.Info("ServiceInstance Saved:{0}",Json.Stringify(m));
			});
		}
		protected override async Task OnNewModel(ModifyContext ctx)
		{
			UIEnsure.NotNull(ctx.Model.Id, "δ���÷���ʵ��ID");
			ctx.Model.CreatedTime = TimeService.Now;
			await base.OnNewModel(ctx);
		}
		protected override async Task OnRemoveModel(ModifyContext ctx)
		{
			if (ctx.Model.IsDefaultService)
				throw new PublicInvalidOperationException("����ɾ��Ĭ�Ϸ���");

			await base.OnRemoveModel(ctx);
		}

		class Config : IServiceConfig
		{
			public long Id { get; set; }

			public long? ParentId { get; set; }

			public string ServiceType { get; set; }

			public string ImplementType { get; set; }

			public string Setting { get; set; }
		}
		IServiceConfig IServiceConfigLoader.GetConfig(long Id)
		{
			var re = DataSet.QuerySingleAsync(
				si => si.Id == Id,
				si => new Config
				{
					
					Id = Id,
					ServiceType = si.ServiceType,
					ImplementType = si.ImplementType,
					Setting= si.Setting,
					ParentId=si.ParentId
				}
				).Result;

			var svcDef = Metadata.ServicesByTypeName.Get(re.ServiceType);
			if (svcDef == null)
				throw new InvalidOperationException($"�Ҳ�������ķ���{re.ServiceType}");

			var svcImpl = svcDef.Implements.SingleOrDefault(i => i.ImplementType.FullName == re.ImplementType);
			if(svcImpl==null)
				throw new InvalidOperationException($"�Ҳ�������ʵ��{re.ImplementType}");

			return re;
		}

		long? IDefaultServiceLocator.Locate(long? ScopeId,string ServiceType)
		{
			var re= DataSet.QuerySingleAsync(
				si => si.ParentId == ScopeId && si.ServiceType==ServiceType && si.IsDefaultService,
				si => si.Id
				).Result;
			return re == 0 ? null : (long?)re;
		}
	}

}
