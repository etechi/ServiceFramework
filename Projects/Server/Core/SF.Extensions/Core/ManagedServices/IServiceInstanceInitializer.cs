using System;
using System.Threading.Tasks;
using System.Linq;
using System.Reflection;
using SF.Entities;
using System.Collections.Generic;
using SF.Core.ServiceManagement.Management;
using System.Collections;

namespace SF.Core.ServiceManagement
{

	public static class ServiceInstanceInitializerImplement
	{

		class ServiceBuilder<I,T> : IServiceInstanceInitializer<I>
		{
			public ServiceInstanceConfig Config { get; } = new ServiceInstanceConfig();
			public Func<IServiceProvider, long?, ServiceInstanceConfig, Task<long>> Builder { get; }
			long _ServiceId;
			public ServiceBuilder(Func<IServiceProvider, long?, ServiceInstanceConfig, Task<long>> builder)
			{
				Config.Name = typeof(T).Comment().Name;
				this.Builder = builder;
			}
			public async Task<long> Ensure(IServiceProvider ServiceProvider, long? ParentId)
			{	
				if (_ServiceId != 0)
					return _ServiceId;


				var metadata = ServiceProvider.Resolve<IServiceMetadata>();
				var impl = metadata.FindServiceByType(typeof(I)).Implements.SingleOrDefault(i => i.ImplementType == typeof(T));
				if (impl == null)
					throw new ArgumentException($"找不到托管服务实现:{typeof(T)}");

				if(!impl.IsManagedService)
					throw new ArgumentException($"指定服务实现不是托管服务实现:{typeof(T)}");

				_ServiceId = await Builder(ServiceProvider, ParentId, Config);
				return _ServiceId;
			}
		}
		
		static IServiceInstanceInitializer<I> CreateBuilder<I,T>(Func<IServiceProvider, long?, ServiceInstanceConfig,Task<long>> builder)
		{
			return new ServiceBuilder<I,T>(builder);
		}

		static async Task<object> ConfigResolve(
			object config, 
			IServiceProvider ServiceProvider, 
			long? ParentId, 
			HashSet<IServiceInstanceInitializer> Children,
			int Level)
		{
			if (Level > 20)
				throw new InvalidOperationException("配置结构太深，不能超过20级");


			if (config == null)
				return null;
			var type = config.GetType();
			if (Type.GetTypeCode(type) != TypeCode.Object)
				return config;
			if (type.IsArray)
			{
				var arr = (Array)config;
				var re = new object[arr.Length];
				for (var i = 0; i < arr.Length; i++)
					re[i] = await ConfigResolve(arr.GetValue(i), ServiceProvider, ParentId, Children,Level+1);
				return re;
			}
			if (config is IServiceInstanceInitializer)
			{
				var sb = (IServiceInstanceInitializer)config;
				var sid = await sb.Ensure(ServiceProvider, ParentId);
				return sid;
			}
			var enumerable = config as IEnumerable;
			if(enumerable!=null)
			{
				var re = new List<object>();
				foreach(var v in enumerable)
					re.Add(await ConfigResolve(v, ServiceProvider, ParentId, Children, Level + 1));
				return re.ToArray();
			}
			var props = config.GetType().GetProperties(
				BindingFlags.Public |
				BindingFlags.Instance |
				BindingFlags.GetProperty |
				BindingFlags.FlattenHierarchy
				);
			var dic = new Dictionary<string, object>();
			foreach (var prop in props)
				dic[prop.Name] = await ConfigResolve(prop.GetValue(config), ServiceProvider, ParentId, Children, Level + 1);
			return dic;
		}
		public static T Config<T>(this T sii, Action<ServiceInstanceConfig> edit)
			where T: IServiceInstanceInitializer
		{
			edit(sii.Config);
			return sii;
		}
		public static T WithDisplay<T>(this T sii, string Name,string Description=null, string Title = null)
			where T: IServiceInstanceInitializer
		{
			sii.Config.Name = Name;
			sii.Config.Title = Title??Name;
			sii.Config.Description = Description;
			return sii;
		}
		
		public static T WithIdent<T>(this T sii, string Ident)
			where T: IServiceInstanceInitializer
		{
			sii.Config.Ident = Ident;
			return sii;
		}
		public static T WithLogicState<T>(this T sii, EntityLogicState State)
			where T : IServiceInstanceInitializer
		{
			sii.Config.LogicState = State;
			return sii;
		}
		public static T Enabled<T>(this T sii)
			where T: IServiceInstanceInitializer
		{
			sii.Config.LogicState = EntityLogicState.Enabled;
			return sii;
		}
		public static IServiceInstanceInitializer<I> DefaultService<I, T>(
			this IServiceInstanceManager manager,
			object cfg,
			params IServiceInstanceInitializer[] childServices
			) where T : I
			=> manager.DefaultServiceWithIdent<I, T>(
			 null,
			 cfg ,
			 childServices
 			);

		public static IServiceInstanceInitializer<I> DefaultServiceWithIdent<I, T>(
			this IServiceInstanceManager manager,
			string ServiceIdent,
			object cfg,
			params IServiceInstanceInitializer[] childServices
			) where T : I
			=>
			manager.CreateService<I, T>(
				 async parent => (await manager.TryGetDefaultService<I>(parent, ServiceIdent))?.Id ??0,
				(parent, rcfg,scfg) => manager.EnsureDefaultService<I, T>(
					parent, 
					rcfg, 
					Name:scfg.Name,
					Title:scfg.Title,
					Description:scfg.Description,
					ServiceIdent:scfg.Ident,
					State: scfg.LogicState
					),
				cfg,
				childServices
				);
		public static IServiceInstanceInitializer<I> Service<I, T>(
			this IServiceInstanceManager manager,
			object cfg,
			params IServiceInstanceInitializer[] childServices
			) where T:I
			=>
			manager.CreateService<I, T>(
				async parent => (await manager.TryGetService<I, T>(parent))?.Id??0,
				(parent, rcfg,scfg) => manager.TryAddService<I, T>(
					parent, 
					rcfg,
					State: EntityLogicState.Disabled,
					Name:scfg.Name,
					Description:scfg.Description,
					ServiceIdent:scfg.Ident,
					Title:scfg.Title
					),
				cfg,
				childServices
				);

		static IServiceInstanceInitializer<I> CreateService<I, T>(
			this IServiceInstanceManager manager,
			Func<long?, Task<long>> FindService,
			Func<long?, object, ServiceInstanceConfig, Task<Models.ServiceInstanceEditable>> ServiceCreator,
			object cfg,
			IServiceInstanceInitializer[] childServices
			) where T : I
		{
			if (childServices != null && childServices.Any(c => c == null))
				throw new ArgumentNullException($"{typeof(I)}的服务{typeof(T)}使用的子服务不能为空");
			return CreateBuilder<I,T>(
				async (sp, parent,scfg) =>
				{

					var svcId = await FindService(parent);
					if(svcId==0)
						svcId = (await ServiceCreator(parent, new { }, scfg)).Id;

					var children = new HashSet<IServiceInstanceInitializer>(childServices);
					foreach (var chd in children)
						await chd.Ensure(sp, svcId);

					var rcfg = await ConfigResolve(cfg, sp, svcId, children,0);
					var nsvcId = (await ServiceCreator(parent, cfg, scfg)).Id;
					if (nsvcId != svcId)
						throw new InvalidOperationException($"服务初始化{typeof(T)}@{typeof(I)}返回ID不一致：第一次：{svcId},第二次：{nsvcId}");

					await manager.SetEntityState(ObjectKey.From(svcId), EntityLogicState.Enabled);
					return svcId;


					//var nsvcId = svcId == 0 ? (long?)null : svcId;
					////var fpr = await sim.ResolveDefaultService<IFilePathResolver>();
					////var fc = await sim.ResolveDefaultService<IFileCache>();
					//var children = new HashSet<IServiceInstanceInitializer>(childServices);
					//foreach (var chd in children)
					//	await chd.Ensure(sp, nsvcId);

					//var rcfg = await ConfigResolve(cfg, sp, nsvcId, children,0);

					//var ms = await ServiceCreator(parent, rcfg);

					//foreach (var chd in children)
					//{
					//	var cid = await chd.Ensure(sp, ms.Id);
					//	await manager.SetServiceParent(cid, ms.Id);
					//}
					//return ms.Id;
				});
		}
		public static IServiceCollection InitServices(
			this IServiceCollection sc,
			string Name,
			Func<IServiceProvider,IServiceInstanceManager, long?,Task> initializer,
			long? ParentId=null
			)
		{
			sc.AddInitializer("service",Name, (sp) =>
			{
				Task.Run(async () =>
				{
					var sim = sp.Resolve<IServiceInstanceManager>();
					await initializer(sp, sim, ParentId);
				}).Wait();
			});
			return sc;
		}
		public static IServiceCollection InitService(
			this IServiceCollection sc,
			string Name,
			Func<IServiceProvider, IServiceInstanceManager, IServiceInstanceInitializer> initializer,
			long? ParentId=null
			)
		{
			return sc.InitServices(
				Name,
				async (sp, sim, parent) =>
				{
					var sii = initializer(sp, sim);
					await sii.Ensure(sp, ParentId);
				});
		}
		public static IServiceCollection InitDefaultService<I,T>(
			this IServiceCollection sc,
			string Name,
			object Config,
			long? ParentId=null
			) where T : I =>
			sc.InitService(
				Name ?? "初始化"+typeof(T).Comment().Name,
				(sp, sim) =>
					sim.DefaultService<I,T>(Config),
				ParentId
				);
		public static IServiceCollection InitDefaultService<I, T>(
			this IServiceCollection sc,
			object Config,
			long? ParentId=null
			) where T : I => sc.InitDefaultService<I, T>(null, Config,ParentId);

		public static async Task<T> TestService<I,T>(
			this IServiceProvider ServiceProvider,
			Func<IServiceInstanceManager, IServiceInstanceInitializer<I>> newInitializer,
			Func<I, Task<T>> Action,
			Func<IServiceProvider, IServiceInstanceManager, long,Task> SetupService=null,
			long? ParentId = null
			) where I:class
		{
			var sim = ServiceProvider.Resolve<IServiceInstanceManager>();
			var initializer = newInitializer(sim);
			var sid = await initializer.Ensure(ServiceProvider, ParentId);
			if(SetupService!=null)
				await SetupService(ServiceProvider, sim, sid);
			var svc=ServiceProvider.Resolve<I>(sid);
			var re=await Action(svc);
			await sim.RemoveAsync(ObjectKey.From(sid));
			return re;
		}

		public static IServiceCollection AddServiceInstanceInitializer<I>(
			this IServiceCollection sc,
			Func<IServiceInstanceManager,IServiceInstanceInitializer<I>> serviceCreater
			)
		{
			return sc.AddTransient(
				sp =>
				serviceCreater(sp.Resolve<IServiceInstanceManager>())
				);
		}
	}

}
