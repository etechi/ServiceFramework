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
using SF.Sys.Services.Internals;
using SF.Sys.Entities;
using SF.Sys.Services.Management.Models;
using SF.Sys.Collections.Generic;
using SF.Sys.Logging;
using SF.Sys.Reflection;
using SF.Sys.Comments;
using SF.Sys.Data;
using SF.Sys.Events;

namespace SF.Sys.Services.Management
{
	/// <summary>
	/// Ĭ�Ϸ���ʵ������
	/// </summary>
	public class ServiceInstanceManager :
		ModidifiableEntityManager<ObjectKey<long>,ServiceInstanceInternal, ServiceInstanceQueryArgument, ServiceInstanceEditable, DataModels.DataServiceInstance>,
		IServiceInstanceManager
	// IServiceConfigLoader,
	//IServiceInstanceLister
	{
		Lazy<IServiceProvider> ServiceProvider { get; }
		IServiceMetadata Metadata { get; }

		public ServiceInstanceManager(
			Lazy<IServiceProvider> ServiceProvider,
			IServiceMetadata Metadata,
			IEntityServiceContext ServiceContext
			) :base(ServiceContext)
		{
			this.ServiceProvider = ServiceProvider;
			this.Metadata = Metadata;
		}

		protected override PagingQueryBuilder<DataModels.DataServiceInstance> PagingQueryBuilder =>
			new PagingQueryBuilder<DataModels.DataServiceInstance>(
				"name",
				b => b.Add("name", i => i.Name));


		protected override async Task<Models.ServiceInstanceEditable> OnMapModelToEditable(IDataContext DataContext, IContextQueryable<DataModels.DataServiceInstance> Query)
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
				throw new InvalidOperationException($"�Ҳ�������ķ���{re.ServiceId}:{re.ServiceType}");
			re.ServiceName = svcDef.ServiceType.Comment().Title;
			re.ServiceType = svcDef.ServiceType.GetFullName();

			var svcImpl = Metadata.ImplementsById.Get(re.ImplementId);
			if (svcImpl == null)
				throw new InvalidOperationException($"�Ҳ�������ʵ��{re.ImplementId}:{re.ImplementType},����:{re.ServiceType}");
			re.ImplementType = svcImpl.ImplementType.GetFullName();
			re.ImplementName = svcImpl.ImplementType.Comment().Title;


			re.SettingType = re.ImplementType + "SettingType";

			Logger.Info("Load ServiceInstance for Edit: {0}",Json.Stringify(re));
			return re;
		}

		protected override IContextQueryable<Models.ServiceInstanceInternal> OnMapModelToDetail(IContextQueryable<DataModels.DataServiceInstance> Query)
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

		protected override IContextQueryable<DataModels.DataServiceInstance> OnBuildQuery(IContextQueryable<DataModels.DataServiceInstance> Query, ServiceInstanceQueryArgument Arg)
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

		
		
		IServiceFactory TestConfig(
			long Id, 
			long? ParentId,
			string ServiceType, 
			string ImplementType, 
			string CreateArguments
			)
		{
			var ServiceResolver = this.ServiceProvider.Value.Resolver();
			using (ServiceResolver.WithScopeService(Id))
			{
				var (decl, impl) = PublicServiceFactory.ResolveMetadata(
					ServiceResolver,
					Id,
					ServiceType,
					ImplementType,
					null
					);

				var factory = PublicServiceFactory.Create(
					Id,
					ParentId,
					()=>null,
					decl,
					impl,
					decl.ServiceType,
					ServiceResolver.Resolve<IServiceMetadata>(),
					string.IsNullOrWhiteSpace(CreateArguments) ? "{}" : CreateArguments
					);
				var svr = factory.Create(ServiceResolver);
				if (svr == null)
					throw new PublicArgumentException("��������ʧ��,���������Ƿ�����");
				var disp = svr as IDisposable;
				if (disp != null)
					disp.Dispose();
				return factory;
			}
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
				m.ServiceId = e.ServiceId;
				m.ServiceType = e.ServiceType;
				m.ImplementType = e.ImplementType;
				m.ImplementId = e.ImplementId;
			}
			else
			{
				UIEnsure.Equal(m.ServiceType, e.ServiceType,"�������Ͳ����޸�");
				UIEnsure.Equal(m.ImplementType, e.ImplementType, "����ʵ�����Ͳ����޸�");
			}

			m.Update(e, Now);

			m.Setting = e.Setting;
			IServiceFactory factory = null;
			if (m.LogicState == EntityLogicState.Enabled)
			{
				factory = TestConfig(m.Id, m.ContainerId, m.ServiceType, m.ImplementType, m.Setting);
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
				ctx.DataContext.AddCommitTracker(
					TransactionCommitNotifyType.AfterCommit,
					async (t,ex) =>
					{
						await ServiceContext.EventEmitService.Emit(
							new InternalServiceChanged
							{
								Time =ServiceContext.Now,
								ScopeId = orgParentId,
								ServiceType = m.ServiceType
							});
						await ServiceContext.EventEmitService.Emit(
							new InternalServiceChanged
							{
								Time = ServiceContext.Now,
								ScopeId = e.ContainerId,
								ServiceType = m.ServiceType
							}
							);
					}
				);
			}
			

			if (await ctx.DataContext.Set<DataModels.DataServiceInstance>().ModifyPosition(
				m,
				PositionModifyAction.Insert,
				i => i.ContainerId == m.ContainerId && i.ServiceType==m.ServiceType,
				i => i.Id == m.Id,
				i => i.ItemOrder,
				(i, p) => i.ItemOrder = p
				) || m.ServiceIdent != e.ServiceIdent)
				ctx.DataContext.AddCommitTracker(
					TransactionCommitNotifyType.AfterCommit,
					async (t, ex) =>
					{
						await ServiceContext.EventEmitService.Emit(
							new InternalServiceChanged
							{
								ScopeId = m.ContainerId,
								ServiceType = m.ServiceType
							});
					});
			m.ServiceIdent = e.ServiceIdent;

			ctx.DataContext.AddCommitTracker(
				TransactionCommitNotifyType.AfterCommit,
				async (t, ex) =>
				{
					await ServiceContext.EventEmitService.Emit(
						new ServiceInstanceChanged
						{
							Id = m.Id
						}
					);
					Logger.Info("ServiceInstance Saved:{0}", Json.Stringify(m));
				});
		}
		protected override async Task OnNewModel(IModifyContext ctx)
		{
			//UIEnsure.NotNull(ctx.Model.Id, "δ���÷���ʵ��ID");
			ctx.Model.Id = await IdentGenerator.GenerateAsync<DataModels.DataServiceInstance>();
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
			//throw new PublicInvalidOperationException("����ɾ��Ĭ�Ϸ���");

			await ServiceContext.RemoveAllAsync<ObjectKey<long>,ServiceInstanceEditable, DataModels.DataServiceInstance>(
				RemoveAsync,
				q => q.ContainerId == ctx.Model.Id
				);

			//var (implTypeName, svcTypeName) = ctx.Model.ImplementType.Split2('@');
			var ServiceResolver = this.ServiceProvider.Value.Resolver();

			var (decl, impl) = PublicServiceFactory.ResolveMetadata(
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
