#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0

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
	[Comment("默认服务实例管理")]
	public class ServiceInstanceManager :
		ModidifiableEntityManager<ObjectKey<long>,ServiceInstanceInternal, ServiceInstanceQueryArgument, ServiceInstanceEditable, DataModels.ServiceInstance>,
		IServiceInstanceManager
	// IServiceConfigLoader,
	//IServiceInstanceLister
	{
		Lazy<IServiceProvider> ServiceProvider { get; }
		IServiceMetadata Metadata { get; }

		public ServiceInstanceManager(
			Lazy<IServiceProvider> ServiceProvider,
			IServiceMetadata Metadata,
			IDataSetEntityManager<ServiceInstanceEditable,DataModels.ServiceInstance> Manager
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
				ServiceId = i.ServiceId,
				ServiceType=i.ServiceType,
				ImplementId = i.ImplementId,
				ImplementType=i.ImplementType,
				Setting= i.Setting
			}).SingleOrDefaultAsync();
			if (re == null)
				return null;

			var svcDef = Metadata.ServicesById.Get(re.ServiceId);
			if (svcDef == null)
				throw new InvalidOperationException($"找不到定义的服务{re.ServiceId}:{re.ServiceType}");
			re.ServiceName = svcDef.ServiceType.Comment().Name;
			re.ServiceType = svcDef.ServiceType.GetFullName();

			var svcImpl = Metadata.ImplementsById.Get(re.ImplementId);
			if (svcImpl == null)
				throw new InvalidOperationException($"找不到服务实现{re.ImplementId}:{re.ImplementType},服务:{re.ServiceType}");
			re.ImplementType = svcImpl.ImplementType.GetFullName();
			re.ImplementName = svcImpl.ImplementType.Comment().Name;


			re.SettingType = re.ImplementType + "SettingType";

			Logger.Info("Load ServiceInstance for Edit: {0}",Json.Stringify(re));
			return re;
		}

		protected override IContextQueryable<Models.ServiceInstanceInternal> OnMapModelToDetail(IContextQueryable<DataModels.ServiceInstance> Query)
		{
			return Query.SelectUIObjectEntity(i => new Models.ServiceInstanceInternal
			{
				Id=i.Id,
				ItemOrder = i.ItemOrder,
				ServiceIdent=i.ServiceIdent,

				ServiceId = i.ServiceId,
				ServiceType = i.ServiceType,

				ImplementId=i.ImplementId,
				ImplementType = i.ImplementType,

				ContainerId=i.ContainerId,
				ContainerName = i.ContainerId.HasValue?i.Container.Name:null
			});
		}

		protected override IContextQueryable<DataModels.ServiceInstance> OnBuildQuery(IContextQueryable<DataModels.ServiceInstance> Query, ServiceInstanceQueryArgument Arg, Paging paging)
		{
			if (Arg.Id != null)
				return Query.Filter(Arg.Id, i => i.Id);
			else
			{
				var defService = Arg.IsDefaultService;
				if (Arg.ServiceIdent != null)
					defService = null;

				Query = Query
					.Filter(Arg.Name, i => i.Name)
					.Filter(Arg.ServiceId, i => i.ServiceId)
					.FilterContains(Arg.ServiceType, i => i.ServiceType)
					.Filter(Arg.ServiceIdent, i => i.ServiceIdent)
					.Filter(Arg.ImplementId, i => i.ImplementId)
					.FilterContains(Arg.ImplementType, i => i.ImplementType)
					.Filter(defService.HasValue ? (defService.Value ? (int?)0 : (int?)-1) : null, i => i.ItemOrder)
				;
				if (Arg.ContainerId.HasValue)
					if (Arg.ContainerId.Value == 0)
						Query = Query.Where(i => i.ContainerId == null);
					else
						Query = Query.Where(i => i.ContainerId == Arg.ContainerId.Value);
				return Query;
			}
		}

		
		
		IServiceFactory TestConfig(long Id, long? ParentId,string ServiceType, string ImplementType, string CreateArguments)
		{
			var ServiceResolver = this.ServiceProvider.Value.Resolver();
			using (ServiceResolver.WithScopeService(Id))
			{
				var (decl, impl) = ServiceFactory.ResolveMetadata(
					ServiceResolver,
					Id,
					ServiceType,
					ImplementType,
					null
					);

				var factory = ServiceFactory.Create(
					Id,
					ParentId,
					new Lazy<long?>(()=>null),
					decl,
					impl,
					decl.ServiceType,
					null,
					ServiceResolver.Resolve<IServiceMetadata>(),
					string.IsNullOrWhiteSpace(CreateArguments) ? "{}" : CreateArguments
					);

				var svr = factory.Create(ServiceResolver);
				if (svr == null)
					throw new PublicArgumentException("创建服务失败,请检查配置是否有误");
				return factory;
			}
		}
		protected override async Task OnUpdateModel(IModifyContext ctx)
		{
			var e = ctx.Editable;
			var m = ctx.Model;
			UIEnsure.HasContent(e.ImplementType,"服务实现");
			UIEnsure.HasContent(e.ServiceType,"服务定义");
			UIEnsure.HasContent(e.Name,"服务名称");
			UIEnsure.HasContent(e.Title, "服务标题");

			if (ctx.Action == ModifyAction.Create)
			{
				m.ServiceId = e.ServiceId;
				m.ServiceType = e.ServiceType;
				m.ImplementType = e.ImplementType;
				m.ImplementId = e.ImplementId;
			}
			else
			{
				UIEnsure.Equal(m.ServiceType, e.ServiceType,"服务类型不能修改");
				UIEnsure.Equal(m.ImplementType, e.ImplementType, "服务实现类型不能修改");
			}

			m.Update(e, Now);

			m.Setting = e.Setting;

			if (m.LogicState == EntityLogicState.Enabled)
			{
				var factory = TestConfig(m.Id, m.ContainerId, m.ServiceType, m.ImplementType, m.Setting);
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
				EntityManager.AddPostAction(async () =>
				{
					await EntityManager.EventEmitter.Emit(
						new InternalServiceChanged
						{
							ScopeId = orgParentId,
							ServiceType = m.ServiceType
						});
					await EntityManager.EventEmitter.Emit(
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
			

			if (await DataSet.ModifyPosition(
				m,
				PositionModifyAction.Insert,
				i => i.ContainerId == m.ContainerId && i.ServiceType==m.ServiceType,
				i => i.Id == m.Id,
				i => i.ItemOrder,
				(i, p) => i.ItemOrder = p
				) || m.ServiceIdent != e.ServiceIdent)
				EntityManager.AddPostAction(async () =>
					await EntityManager.EventEmitter.Emit(
						new InternalServiceChanged
						{
							ScopeId = m.ContainerId,
							ServiceType = m.ServiceType
						})
					,
					PostActionType.AfterCommit);
			m.ServiceIdent = e.ServiceIdent;

			EntityManager.AddPostAction(async () =>
			{
				await EntityManager.EventEmitter.Emit(
					new ServiceInstanceChanged
					{
						Id = m.Id
					}
				);
				Logger.Info("ServiceInstance Saved:{0}", Json.Stringify(m));
			},
			PostActionType.AfterCommit);
		}
		protected override async Task OnNewModel(IModifyContext ctx)
		{
			//UIEnsure.NotNull(ctx.Model.Id, "未设置服务实例ID");
			ctx.Model.Id = await IdentGenerator.GenerateAsync();
			ctx.Model.Create(Now);
		}

		class InstanceDescriptor : IServiceInstanceDescriptor
		{
			public long InstanceId { get; set; }

			public long? ParentInstanceId { get; set; }
			public long? DataScopeId { get; set; }

			public bool IsManaged { get; set; }

			public IServiceDeclaration ServiceDeclaration { get; set; }

			public IServiceImplement ServiceImplement { get; set; }

			public IDisposable OnSettingChanged<T>(Action<T> Callback)
			{
				return Disposable.Empty;
			}
		}
		protected override async Task OnRemoveModel(IModifyContext ctx)
		{
			//if (ctx.Model.IsDefaultService)
			//throw new PublicInvalidOperationException("不能删除默认服务");

			await EntityManager.RemoveAllAsync<ObjectKey<long>,ServiceInstanceEditable, DataModels.ServiceInstance>(
				RemoveAsync,
				q => q.ContainerId == ctx.Model.Id
				);

			//var (implTypeName, svcTypeName) = ctx.Model.ImplementType.Split2('@');
			var ServiceResolver = this.ServiceProvider.Value.Resolver();

			var (decl, impl) = ServiceFactory.ResolveMetadata(
				ServiceResolver,
				ctx.Model.Id,
				ctx.Model.ServiceType,
				ctx.Model.ImplementType,
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
