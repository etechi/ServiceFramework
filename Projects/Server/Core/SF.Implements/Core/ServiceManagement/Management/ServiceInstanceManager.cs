using System;
using System.Linq;
using System.Threading.Tasks;
using SF.Auth;
using SF.Data;
using SF.Entities;
using SF.Metadata;
using SF.Core.ServiceManagement.Models;
using SF.Core.ServiceManagement.Internals;
using SF.Core.Logging;
using System.Collections.Generic;
using SF.Core.Times;
using System.Reflection;

namespace SF.Core.ServiceManagement.Management
{
	[Comment("Ĭ�Ϸ���ʵ������")]
	public class ServiceInstanceManager :
		EntityManager<long, ServiceInstanceInternal, ServiceInstanceQueryArgument, ServiceInstanceEditable, DataModels.ServiceInstance>,
		IServiceInstanceManager
	// IServiceConfigLoader,
	//IServiceInstanceLister
	{
		Lazy<IServiceProvider> ServiceProvider { get; }
		IServiceMetadata Metadata { get; }
		IDataSetEntityManager<DataModels.ServiceInstance> Manager { get; }

		public ServiceInstanceManager(
			Lazy<IServiceProvider> ServiceProvider,
			IServiceMetadata Metadata,
			IDataSetEntityManager<DataModels.ServiceInstance> Manager
			):base(Manager)
		{
			this.ServiceProvider = ServiceProvider;
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
				ItemOrder = i.ItemOrder,
				ServiceIdent=i.ServiceIdent,
				ContainerId=i.ContainerId,
				ContainerName=i.ContainerId.HasValue?i.Container.Name:null,
				ServiceType = i.ServiceType,
				ImplementType = i.ImplementType,
				Setting= i.Setting
			}).SingleOrDefaultAsync();
			if (re == null)
				return null;

			var svcDef = Metadata.ServicesByTypeName.Get(re.ServiceType);
			if (svcDef == null)
				throw new InvalidOperationException($"�Ҳ�������ķ���{re.ServiceType}");
			re.ServiceName = svcDef.ServiceType.Comment().Name;
			var (implType, _) = re.ImplementType.Split2('@');
			var svcImpl = svcDef.Implements.SingleOrDefault(i => i.ImplementType.GetFullName() == implType);
			if (svcImpl == null)
				throw new InvalidOperationException($"�Ҳ�������ʵ��{re.ImplementType}");
			re.ImplementName = svcImpl.ImplementType.Comment().Name;


			re.SettingType = implType + "SettingType";

			Manager.Logger.Info("Load ServiceInstance for Edit: {0}",Json.Stringify(re));
			return re;
		}

		protected override IContextQueryable<Models.ServiceInstanceInternal> OnMapModelToPublic(IContextQueryable<DataModels.ServiceInstance> Query)
		{
			return Query.SelectUIObjectEntity(i => new Models.ServiceInstanceInternal
			{
				Id=i.Id,
				ServiceType = i.ServiceType,
				ItemOrder = i.ItemOrder,
				ServiceIdent=i.ServiceIdent,
				ServiceName = i.ServiceType,
				ImplementType = i.ImplementType,
				ContainerId=i.ContainerId,
				ContainerName = i.ContainerId.HasValue?i.Container.Name:null
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
					.Filter(Arg.ContainerId,i=>i.ContainerId)
					.Filter(Arg.IsDefaultService.HasValue? (Arg.IsDefaultService.Value?(int?)0:(int?)-1):null,i=>i.ItemOrder)
				;
		}

		
		
		IServiceFactory TestConfig(long Id, long? ParentId, string ImplementId, string CreateArguments)
		{
			var (implTypeName, svcTypeName) = ImplementId.Split2('@');
			var ServiceResolver = this.ServiceProvider.Value.Resolver();

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
				throw new PublicArgumentException("��������ʧ��,���������Ƿ�����");
			return factory;		

		}
		protected override async Task OnUpdateModel(IModifyContext ctx)
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

			m.Update(e, Manager.Now);

			m.Setting = e.Setting;

			if (m.LogicState == EntityLogicState.Enabled)
			{
				var factory = TestConfig(m.Id, m.ContainerId, m.ImplementType, m.Setting);
				if (factory.ServiceImplement.ManagedServiceInitializer != null)
					await factory.ServiceImplement.ManagedServiceInitializer.Init(
						ServiceProvider.Value,
						factory
						);
			}
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

			if (m.ContainerId != e.ContainerId)
			{
				var orgParentId = m.ContainerId;
				m.ContainerId = e.ContainerId;
				Manager.AddPostAction(async () =>
				{
					await Manager.EventEmitter.Emit(
						new InternalServiceChanged
						{
							ScopeId = orgParentId,
							ServiceType = m.ServiceType
						});
					await Manager.EventEmitter.Emit(
						new InternalServiceChanged
						{
							ScopeId = e.ContainerId,
							ServiceType = m.ServiceType
						}
						);
				},
				PostActionType.AfterCommit
				);
			}
			

			if (await Manager.DataSet.ModifyPosition(
				m,
				PositionModifyAction.Insert,
				i => i.ContainerId == m.ContainerId && i.ServiceType==m.ServiceType,
				i => i.Id == m.Id,
				i => i.ItemOrder,
				(i, p) => i.ItemOrder = p
				) || m.ServiceIdent != e.ServiceIdent)
				Manager.AddPostAction(async () =>
					await Manager.EventEmitter.Emit(
						new InternalServiceChanged
						{
							ScopeId = m.ContainerId,
							ServiceType = m.ServiceType
						})
					,
					PostActionType.AfterCommit);
			m.ServiceIdent = e.ServiceIdent;

			Manager.AddPostAction(async () =>
			{
				await Manager.EventEmitter.Emit(
					new ServiceInstanceChanged
					{
						Id = m.Id
					}
				);
				Manager.Logger.Info("ServiceInstance Saved:{0}", Json.Stringify(m));
			},
			PostActionType.AfterCommit);
		}
		protected override async Task OnNewModel(IModifyContext ctx)
		{
			//UIEnsure.NotNull(ctx.Model.Id, "δ���÷���ʵ��ID");
			ctx.Model.Id = await Manager.IdentGenerator.GenerateAsync("ϵͳ����", 0);
			ctx.Model.Create(Manager.Now);
		}

		class InstanceDescriptor : IServiceInstanceDescriptor
		{
			public long InstanceId { get; set; }

			public long? ParentInstanceId { get; set; }

			public bool IsManaged { get; set; }

			public IServiceDeclaration ServiceDeclaration { get; set; }

			public IServiceImplement ServiceImplement { get; set; }
		}
		protected override async Task OnRemoveModel(IModifyContext ctx)
		{
			//if (ctx.Model.IsDefaultService)
			//throw new PublicInvalidOperationException("����ɾ��Ĭ�Ϸ���");

			await Manager.RemoveAllAsync<long,DataModels.ServiceInstance>(
				RemoveAsync,
				q => q.ContainerId == ctx.Model.Id
				);

			var (implTypeName, svcTypeName) = ctx.Model.ImplementType.Split2('@');
			var ServiceResolver = this.ServiceProvider.Value.Resolver();

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
						ParentInstanceId = ctx.Model.ContainerId,
						IsManaged = true,
						ServiceDeclaration = decl,
						ServiceImplement = impl
					});
		}

	}
}
