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

namespace SF.Core.ServiceManagement.Management
{
	[Comment("默认服务实例管理")]
	public class ServiceInstanceManager :
		EntityManager<string, Models.ServiceInstanceInternal, ServiceInstanceQueryArgument, Models.ServiceInstanceEditable, DataModels.ServiceInstance>,
		IServiceInstanceManager,
		 IServiceConfigLoader, 
		IDefaultServiceLocator
	{
		IDataEntityResolver EntityResolver { get;}
		IServiceProvider ServiceProvider { get; }
		public IServiceInstanceConfigChangedNotifier ConfigChangedNotifier { get; }
		ILogger<ServiceInstanceManager> Logger { get; }
		public ServiceInstanceManager(
			IDataSet<DataModels.ServiceInstance> DataSet, 
			IDataEntityResolver EntityResolver,
			IServiceInstanceConfigChangedNotifier ConfigChangedNotifier,
			IServiceProvider ServiceProvider,
			ILogger<ServiceInstanceManager> Logger,
			IServiceMetadata ServiceMetadata
			) : 
			base(DataSet)
		{
			this.ServiceProvider = ServiceProvider;
			this.EntityResolver = EntityResolver;
			this.ConfigChangedNotifier = ConfigChangedNotifier;
			this.Logger = Logger;
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
				DeclarationId = i.DeclarationId,
				LogicState = i.LogicState,
				IsDefaultService=i.IsDefaultService,
				Name = i.Name,
				Title = i.Title,
				Description=i.Description,
				Image=i.Image,
				Icon=i.Icon,
				Remarks=i.Remarks,
				ImplementId=i.ImplementId,
				Setting=i.Setting
			}).SingleAsync();

			re.SettingType = re.ImplementName + "SettingType";

			Logger.Info("Load ServiceInstance for Edit: {0}",Json.Stringify(re));
			return re;
		}

		protected override IContextQueryable<Models.ServiceInstanceInternal> OnMapModelToPublic(IContextQueryable<DataModels.ServiceInstance> Query)
		{
			return  Query.Select(i => new Models.ServiceInstanceInternal
			{
				Id = i.Id,
				DeclarationId = i.DeclarationId,
				//ImplementId = i.ImplementId,
				LogicState = i.LogicState,
				IsDefaultService=i.IsDefaultService,
				Name = i.Name,
				Title=i.Title,
				Description=i.Description,
				Icon=i.Icon,
				Image=i.Image
			});
		}

		protected override IContextQueryable<DataModels.ServiceInstance> OnBuildQuery(IContextQueryable<DataModels.ServiceInstance> Query, ServiceInstanceQueryArgument Arg, Paging paging)
		{
			if (Arg.Id.HasValue)
				return Query.Filter(Arg.Id.Value, i => i.Id);
			else
				return Query
					.Filter(Arg.Name, i => i.Name)
					.Filter(Arg.DeclarationId, i => i.DeclarationId)
					//.Filter(Arg.ImplementId, i => i.ImplementId)
					.Filter(Arg.IsDefaultService,i=>i.IsDefaultService)
				;
		}

		protected override async Task<Models.ServiceInstanceInternal[]> OnPreparePublics(Models.ServiceInstanceInternal[] Internals)
		{
			await EntityResolver.Fill(
				Internals,
				i => new[] { "系统服务实现-" + i.ImplementId, "系统服务定义-" + i.DeclarationId },
				(i,ns)=> {
					i.ImplementName=ns[0];
					i.DeclarationName=ns[1];
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
			UIEnsure.HasContent(e.ImplementId,"服务实现");
			UIEnsure.HasContent(e.DeclarationId,"服务定义");
			UIEnsure.HasContent(e.Name,"服务名称");
			UIEnsure.HasContent(e.Title, "服务标题");

			m.DeclarationId = e.DeclarationId;
			m.LogicState = e.LogicState;
			m.Name = e.Name;
			m.Title = e.Title;
			m.Description = e.Description;
			m.Icon = e.Icon;
			m.Remarks = e.Remarks;
			m.Image = e.Image;
			m.Setting = e.Setting;

			//TestConfig(m.Id, m.ImplementId, m.CreateArguments);
		
			if (await DataSet.TrySetDefaultItem(
				m,
				e.IsDefaultService,
				i => i.IsDefaultService,
				(i, d) => i.IsDefaultService = d,
				i => i.DeclarationId == m.DeclarationId,
				i => i.Id == m.Id,
				i=>i.IsDefaultService
				))
				ctx.AddPostAction(() => ConfigChangedNotifier.NotifyDefaultChanged(m.DeclarationId));

			ctx.AddPostAction(() =>
			{
				var key = m.Id.Split2('-');
				ConfigChangedNotifier.NotifyChanged(key.Item1,key.Item2.ToInt32());
				Logger.Info("ServiceInstance Saved:{0}",Json.Stringify(m));

			});
		}
		protected override Task OnNewModel(ModifyContext ctx)
		{
			ctx.Model.Id = Guid.NewGuid().ToString("N");
			return base.OnNewModel(ctx);
		}
		protected override async Task OnRemoveModel(ModifyContext ctx)
		{
			if (ctx.Model.IsDefaultService)
				throw new PublicInvalidOperationException("不能删除默认服务");

			await base.OnRemoveModel(ctx);
		}
		class Config : IServiceConfig
		{
			public string ServiceType { get; set; }
			public long Id { get; set; }
			public string ImplementType { get; set; }
			public string Settings { get; set; }
		}
		IServiceConfig IServiceConfigLoader.GetConfig(string ServiceType,long Id)
		{
			var key = ServiceType + "-" + Id;
			var re = DataSet.QuerySingleAsync(
				si => si.Id == key,
				si => new
				{
					id = si.Id,
					cfg =
					new Config
					{
						ServiceType = si.DeclarationId,
						ImplementType = si.ImplementId,
						Settings = si.Setting
					}
				}).Result;
			re.cfg.Id = re.id.Split2('-').Item2.ToInt64();
			return re.cfg;
		}

		long? IDefaultServiceLocator.Locate(string Type)
		{
			var re= DataSet.QuerySingleAsync(
				si => si.DeclarationId==Type && si.IsDefaultService,
				si => si.Id
				).Result;
			return re?.Split2('-').Item2.ToInt32();
		}
	}

}
