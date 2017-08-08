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
	[Comment("默认服务实例管理")]
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
			var re= await Query.SelectUIObjectEntity(i => new Models.ServiceInstanceEditable
			{
				Id = i.Id,
				//ImplementId = i.ImplementId,
				Priority = i.Priority,
				ServiceIdent=i.ServiceIdent,
				ParentId=i.ParentId,
				ParentName=i.ParentId.HasValue?i.Parent.Name:null,
				ServiceType = i.ServiceType,
				ImplementType = i.ImplementType,
				Setting= i.Setting
			}).SingleOrDefaultAsync();
			if (re == null)
				return null;

			var svcDef = Metadata.ServicesByTypeName.Get(re.ServiceType);
			if (svcDef == null)
				throw new InvalidOperationException($"找不到定义的服务{re.ServiceType}");
			re.ServiceName = svcDef.ServiceType.Comment().Name;
			var (implType, _) = re.ImplementType.Split2('@');
			var svcImpl = svcDef.Implements.SingleOrDefault(i => i.ImplementType.GetFullName() == implType);
			if (svcImpl == null)
				throw new InvalidOperationException($"找不到服务实现{re.ImplementType}");
			re.ImplementName = svcImpl.ImplementType.Comment().Name;


			re.SettingType = implType + "SettingType";

			Logger.Info("Load ServiceInstance for Edit: {0}",Json.Stringify(re));
			return re;
		}

		protected override IContextQueryable<Models.ServiceInstanceInternal> OnMapModelToPublic(IContextQueryable<DataModels.ServiceInstance> Query)
		{
			return Query.SelectUIObjectEntity(i => new Models.ServiceInstanceInternal
			{
				Id=i.Id,
				ServiceType = i.ServiceType,
				Priority = i.Priority,
				ServiceIdent=i.ServiceIdent,
				ServiceName = i.ServiceType,
				ImplementType = i.ImplementType,
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
					.Filter(Arg.ParentId,i=>i.ParentId)
					.Filter(Arg.IsDefaultService.HasValue? (Arg.IsDefaultService.Value?(int?)0:(int?)-1):null,i=>i.Priority)
				;
		}

		protected override async Task<Models.ServiceInstanceInternal[]> OnPreparePublics(Models.ServiceInstanceInternal[] Internals)
		{
			await EntityResolver.Fill(
				Internals,
				i => new[] { "系统服务实现-" + i.ImplementType, "系统服务定义-" + i.ServiceType},
				(i,ns)=> {
					i.ImplementName=ns[0];
					i.ServiceName=ns[1];
				});
			return Internals;
		}
		
		IServiceFactory TestConfig(long Id, long? ParentId, string ImplementId, string CreateArguments)
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
				string.IsNullOrWhiteSpace(CreateArguments) ? "{}":CreateArguments
				);

			var svr = factory.Create(ServiceResolver);
			if (svr == null)
				throw new PublicArgumentException("创建服务失败,请检查配置是否有误");
			return factory;		

		}
		protected override async Task OnUpdateModel(ModifyContext ctx)
		{
			var e = ctx.Editable;
			var m = ctx.Model;
			UIEnsure.HasContent(e.ImplementType,"服务实现");
			UIEnsure.HasContent(e.ServiceType,"服务定义");
			UIEnsure.HasContent(e.Name,"服务名称");
			UIEnsure.HasContent(e.Title, "服务标题");

			if (ctx.Action == ModifyAction.Create)
			{
				m.ServiceType = e.ServiceType;
				m.ImplementType = e.ImplementType;
				
			}
			else
			{
				UIEnsure.Equal(m.ServiceType, e.ServiceType,"服务类型不能修改");
				UIEnsure.Equal(m.ImplementType, e.ImplementType, "服务实现类型不能修改");
			}

			m.Update(e, TimeService.Now);

			m.Setting = e.Setting;
			var factory=TestConfig(m.Id,m.ParentId, m.ImplementType, m.Setting);
			if (factory.ServiceImplement.ManagedServiceInitializer != null)
				await factory.ServiceImplement.ManagedServiceInitializer.Init(
					ServiceProvider.Value,
					factory
					);

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

			if (m.ParentId != e.ParentId)
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
			//UIEnsure.NotNull(ctx.Model.Id, "未设置服务实例ID");
			ctx.Model.Id = await IdentGenerator.Value.GenerateAsync("系统服务", 0);
			ctx.Model.Create(TimeService.Now);
			await base.OnNewModel(ctx);
		}

		class InstanceDescriptor : IServiceInstanceDescriptor
		{
			public long InstanceId { get; set; }

			public long? ParentInstanceId { get; set; }

			public bool IsManaged { get; set; }

			public IServiceDeclaration ServiceDeclaration { get; set; }

			public IServiceImplement ServiceImplement { get; set; }
		}
		protected override async Task OnRemoveModel(ModifyContext ctx)
		{
			//if (ctx.Model.IsDefaultService)
			//throw new PublicInvalidOperationException("不能删除默认服务");
			var (implTypeName, svcTypeName) = ctx.Model.ImplementType.Split2('@');
			var ServiceResolver = this.ServiceProvider.Value.NewResolver();

			var (decl, impl) = ServiceFactory.ResolveMetadata(
				ServiceResolver,
				ctx.Model.Id,
				svcTypeName,
				implTypeName,
				null
				);
			if (impl != null && impl.ManagedServiceInitializer != null)
				await impl.ManagedServiceInitializer.Uninit(
					ServiceProvider.Value, 
					new InstanceDescriptor
					{
						InstanceId = ctx.Model.Id,
						ParentInstanceId = ctx.Model.ParentId,
						IsManaged = true,
						ServiceDeclaration = decl,
						ServiceImplement = impl
					});
			await base.OnRemoveModel(ctx);
		}

	}

}
