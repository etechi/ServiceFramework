using System;
using System.Linq;
using System.Threading.Tasks;
using SF.Auth;
using SF.Data;
using SF.Data.Entity;
using SF.Data.Storage;
using SF.Metadata;
using SF.Core.ManagedServices.Admin.DataModels;
using SF.Core.ManagedServices.Models;
using SF.Core.ManagedServices.Runtime;
using SF.Core.DI;
using SF.Core.Logging;

namespace SF.Core.ManagedServices.Admin
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
		public IManagedServiceConfigChangedNotifier ConfigChangedNotifier { get; }
		ILogger<ServiceInstanceManager> Logger { get; }
		public ServiceInstanceManager(
			IDataSet<DataModels.ServiceInstance> DataSet, 
			IDataEntityResolver EntityResolver,
			IManagedServiceConfigChangedNotifier ConfigChangedNotifier,
			IServiceProvider ServiceProvider,
			ILogger<ServiceInstanceManager> Logger
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
				Setting = i.CreateArguments,
				DeclarationId = i.DeclarationId,
				ImplementId = i.ImplementId,
				LogicState = i.LogicState,
				IsDefaultService=i.IsDefaultService,
				Name = i.Name,
				Title = i.Title,
				Description=i.Description,
				Image=i.Image,
				Icon=i.Icon,
				Remarks=i.Remarks
			}).SingleAsync();

			re.SettingType = re.ImplementId.Split2('@').Item1 + "CreateArguments";

			Logger.Info("Load ServiceInstance for Edit: {0}",Json.Stringify(re));
			return re;
		}

		protected override IContextQueryable<Models.ServiceInstanceInternal> OnMapModelToPublic(IContextQueryable<DataModels.ServiceInstance> Query)
		{
			return  Query.Select(i => new Models.ServiceInstanceInternal
			{
				Id = i.Id,
				DeclarationId = i.DeclarationId,
				ImplementId = i.ImplementId,
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
					.Filter(Arg.ImplementId, i => i.ImplementId)
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
		void TestConfig(string Id,string ImplementId,string CreateArguments)
		{
			var ServiceScope = ServiceProvider.Resolve<IManagedServiceScope>();
			var ImplementTypeResolver = ServiceProvider.Resolve<IServiceImplementTypeResolver>();
			var ServiceMetadata = ServiceProvider.Resolve<IServiceMetadata>();
			var ImplementType = ImplementTypeResolver.Resolve(ImplementId.Split2('@').Item1);
			var ci = Runtime.ServiceFactoryBuilder.FindBestConstructorInfo(ImplementType);
			var tmpl = Runtime.ServiceCreateParameterTemplate.Load(
					ci,
					Id,
					CreateArguments,
					ServiceMetadata
					);

			var factory = Runtime.ServiceFactoryBuilder.Build(
				ServiceMetadata,
				ImplementType,
				ci
				);
			var svr=factory(ServiceProvider, ServiceScope, tmpl);
			if (svr == null)
				throw new PublicArgumentException("配置错误,创建服务失败");
		}
		protected override async Task OnUpdateModel(ModifyContext ctx)
		{
			var e = ctx.Editable;
			var m = ctx.Model;
			UIEnsure.HasContent(e.ImplementId,"服务实现");
			UIEnsure.HasContent(e.DeclarationId,"服务定义");
			UIEnsure.HasContent(e.Name,"服务名称");
			UIEnsure.HasContent(e.Title, "服务标题");
			//UIEnsure.HasContent(e.Setting, "服务配置");

			m.CreateArguments = e.Setting;
			m.DeclarationId = e.DeclarationId;
			m.ImplementId = e.ImplementId;
			m.LogicState = e.LogicState;
			m.Name = e.Name;
			m.Title = e.Title;
			m.Description = e.Description;
			m.Icon = e.Icon;
			m.Remarks = e.Remarks;
			m.Image = e.Image;

			TestConfig(m.Id, m.ImplementId, m.CreateArguments);

			if (!m.IsDefaultService)
			{
				var curDefaultEntity = await DataSet.FindAsync(si => si.DeclarationId == m.DeclarationId && si.Id != m.Id && si.IsDefaultService);
				if (curDefaultEntity == null || e.IsDefaultService)
				{
					m.IsDefaultService = true;
					if (curDefaultEntity != null)
					{
						curDefaultEntity.IsDefaultService = false;
						DataSet.Update(curDefaultEntity);
					}

					ctx.AddPostAction(() => ConfigChangedNotifier.NotifyDefaultChanged(m.DeclarationId));
				}
			}
			ctx.AddPostAction(() =>
			{
				ConfigChangedNotifier.NotifyChanged(m.Id);
				Logger.Info("ServiceInstance Saved:{0}",Json.Stringify(m));

			});
		}
		protected override Task OnNewModel(ModifyContext ctx)
		{
			ctx.Model.Id = Guid.NewGuid().ToString("N");
			return base.OnNewModel(ctx);
		}
		protected override Task OnRemoveModel(ModifyContext ctx)
		{
			if (ctx.Model.IsDefaultService)
				throw new PublicInvalidOperationException("不能删除默认服务");
			return base.OnRemoveModel(ctx);
		}
		class Config : IServiceConfig
		{
			public string CreateArguments { get; set; }
			public string ImplementType { get; set; }
			public string ServiceType { get; set; }
		}
		IServiceConfig IServiceConfigLoader.GetConfig(string Id)
		{
			var re = DataSet.QuerySingleAsync(
				si => si.Id == Id,
				si => new Config
				{
					ServiceType = si.DeclarationId,
					ImplementType = si.ImplementId,
					CreateArguments = si.CreateArguments
				}).Result;
			re.ImplementType = re.ImplementType.Split2('@').Item1;
			return re;
		}

		string IDefaultServiceLocator.Locate(string Type)
		{
			return DataSet.QuerySingleAsync(
				si => si.DeclarationId==Type && si.IsDefaultService,
				si => si.Id
				).Result;
		}
	}

}
