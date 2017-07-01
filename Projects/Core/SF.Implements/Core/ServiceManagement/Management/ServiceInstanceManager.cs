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
				DisplayData=new Data.Models.UIDisplayData
				{
					Description=i.Description,
					Icon=i.Icon,
					Image=i.Image,
					Remarks=i.Remarks,
					SubTitle=i.SubTitle
				},
				Interfaces=from ii in i.Interfaces
						   select new ServiceInstanceInterface
						   {
							   ImplementType=ii.ImplementType,
							   InterfaceType=ii.InterfaceType,
							   Setting=ii.Setting
						   }
			}).SingleAsync();

			var svcDef = Metadata.ServicesByTypeName.Get(re.ServiceType);
			if (svcDef == null)
				throw new InvalidOperationException($"找不到定义的服务{re.ServiceType}");

			var svcImpl = svcDef.Implements.SingleOrDefault(i => i.ImplementType.FullName == re.ImplementType);
			if (svcImpl == null)
				throw new InvalidOperationException($"找不到服务实现{re.ImplementType}");
			var curSettings = re.Interfaces.ToDictionary(i => i.InterfaceName + "/" + i.ImplementType);

			re.Interfaces = (
				from i in svcImpl.Interfaces
				let  s = curSettings.Get(i.ServiceType.FullName+"/"+i.ImplementType.FullName)
				select new ServiceInstanceInterface
				{
					ImplementName = i.ImplementType.FullName,
					ImplementType = i.ImplementType.FullName,
					InterfaceName = i.ServiceType.FullName,
					InterfaceType = i.ServiceType.FullName,
					Setting = s?.Setting ?? null,
					SettingType = i.ImplementType + "SettingType"
				});

			Logger.Info("Load ServiceInstance for Edit: {0}",Json.Stringify(re));
			return re;
		}

		protected override IContextQueryable<Models.ServiceInstanceInternal> OnMapModelToPublic(IContextQueryable<DataModels.ServiceInstance> Query)
		{
			return  Query.Select(i => new Models.ServiceInstanceInternal
			{
				Id = i.Id,
				ServiceType = i.ServiceType,
				//ImplementId = i.ImplementId,
				ObjectState = i.ObjectState,
				IsDefaultService=i.IsDefaultService,
				Name = i.Name,
				Title=i.Title,
				CreatedTime=i.CreatedTime,
				ServiceName=i.ServiceType,
				ImplementType=i.ImplementType,
				ImplementName=i.ImplementType,
				UpdatedTime=i.UpdatedTime,
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
				i => new[] { "系统服务实现-" + i.ImplementType, "系统服务定义-" + i.ServiceType},
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
		//		throw new PublicArgumentException("配置错误,创建服务失败");
		//}
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
			m.ObjectState = e.ObjectState;
			m.Name = e.Name;
			m.Title = e.Title;

			m.Description = e?.DisplayData?.Description;
			m.Icon = e?.DisplayData?.Icon;
			m.Remarks = e?.DisplayData?.Remarks;
			m.Image = e?.DisplayData?.Image;

			m.UpdatedTime= TimeService.Now;
			//TestConfig(m.Id, m.ImplementId, m.CreateArguments);
			var siis = DataSet.Context
					.Set<DataModels.ServiceInstanceInterface>();
			var orgs = await siis.LoadListAsync(sii => sii.ServiceInstanceId == m.Id);
			siis.Merge<DataModels.ServiceInstanceInterface,Models.ServiceInstanceInterface>(
				orgs,
				e?.Interfaces,
				(mi, ei) => mi.InterfaceType == ei.InterfaceType,
				ei => new DataModels.ServiceInstanceInterface
				{
					ServiceInstanceId=m.Id,
					InterfaceType=ei.InterfaceType,
					ImplementType=ei.ImplementType,
					Setting=ei.Setting
				},
				(mi, ei) => mi.Setting = ei.Setting
				);

			if (await DataSet.TrySetDefaultItem(
				m,
				e.IsDefaultService,
				i => i.IsDefaultService,
				(i, d) => i.IsDefaultService = d,
				i => i.ServiceType == m.ServiceType,
				i => i.Id == m.Id,
				i=>i.IsDefaultService
				))
				ctx.AddPostAction(() => ConfigChangedNotifier.NotifyDefaultChanged(m.ServiceType,m.AppId));

			ctx.AddPostAction(() =>
			{
				ConfigChangedNotifier.NotifyChanged(m.ServiceType,m.AppId, m.Id);
				Logger.Info("ServiceInstance Saved:{0}",Json.Stringify(m));
			});
		}
		protected override async Task OnNewModel(ModifyContext ctx)
		{
			UIEnsure.NotNull(ctx.Model.Id, "未设置服务实例ID");
			ctx.Model.CreatedTime = TimeService.Now;
			await base.OnNewModel(ctx);
		}
		protected override async Task OnRemoveModel(ModifyContext ctx)
		{
			if (ctx.Model.IsDefaultService)
				throw new PublicInvalidOperationException("不能删除默认服务");

			await base.OnRemoveModel(ctx);
		}

		class InterfaceConfig : IServiceInterfaceConfig
		{
			public string ImplementType { get; set; }

			public string Setting { get; set; }
		}
		class Config : IServiceConfig
		{
			public string ServiceType { get; set; }
			public long Id { get; set; }
			public string ImplementType { get; set; }
			public int AppId { get; set; }
			public IReadOnlyDictionary<string,IServiceInterfaceConfig> Settings { get; set; }
		}
		IServiceConfig IServiceConfigLoader.GetConfig(string ServiceType,int AppId, long Id)
		{
			var re = DataSet.QuerySingleAsync(
				si => si.Id == Id,
				si => new
				{
					cfg = new Config
					{
						Id = Id,
						ServiceType = si.ServiceType,
						ImplementType = si.ImplementType,
						Settings = null,
						AppId=si.AppId
					},
					ifs=from i in si.Interfaces
						select new InterfaceConfig
						{
							ImplementType=i.ImplementType,
							Setting=i.Setting,
							
						}
				}
				).Result;

			var svcDef = Metadata.ServicesByTypeName.Get(re.cfg.ServiceType);
			if (svcDef == null)
				throw new InvalidOperationException($"找不到定义的服务{re.cfg.ServiceType}");

			var svcImpl = svcDef.Implements.SingleOrDefault(i => i.ImplementType.FullName == re.cfg.ImplementType);
			if(svcImpl==null)
				throw new InvalidOperationException($"找不到服务实现{re.cfg.ImplementType}");

			var settings = re.ifs.ToDictionary(i => i.ImplementType);

			re.cfg.Settings = (
				from ii in svcImpl.Interfaces
				let di = settings.Get(ii.ImplementType.FullName)
				select di ?? new InterfaceConfig
				{
					ImplementType = ii.ImplementType.FullName
				}
				).Cast<IServiceInterfaceConfig>().ToDictionary(i => i.ImplementType);

			return re.cfg;
		}

		long IDefaultServiceLocator.Locate(string Type,int AppId)
		{
			return DataSet.QuerySingleAsync(
				si => si.ServiceType == Type && si.AppId==AppId && si.IsDefaultService,
				si => si.Id
				).Result;
		}
	}

}
