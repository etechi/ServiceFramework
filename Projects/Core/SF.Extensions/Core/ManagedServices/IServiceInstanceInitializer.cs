using System;
using System.Threading.Tasks;
using System.Linq;
using System.Reflection;
using SF.Data.Entity;
using System.Collections.Generic;
using SF.Core.ServiceManagement.Management;
using System.Collections;

namespace SF.Core.ServiceManagement
{

	public static class ServiceInstanceInitializerImplement
	{

		class ServiceBuilder<I,T> : IServiceInstanceInitializer<I>
		{
			public string Name { get; }
			public Func<IServiceProvider, long?, Task<long>> Builder { get; }
			long _ServiceId;
			public ServiceBuilder(string Name, Func<IServiceProvider, long?, Task<long>> builder)
			{
				this.Name = Name;
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

				_ServiceId = await Builder(ServiceProvider, ParentId);
				return _ServiceId;
			}
		}
		
		static IServiceInstanceInitializer<I> CreateBuilder<I,T>(string Name, Func<IServiceProvider, long?, Task<long>> builder)
		{
			return new ServiceBuilder<I,T>(Name, builder);
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
		public static IServiceInstanceInitializer<I> DefaultService<I, T>(
			this IServiceInstanceManager manager,
			object cfg,
			params IServiceInstanceInitializer[] childServices
			)
			=>
			manager.CreateService<I, T>(
				 parent => manager.TryGetDefaultService<I>(parent),
				(parent, rcfg) => manager.EnsureDefaultService<I, T>(parent, rcfg, State: Data.LogicObjectState.Disabled),
				cfg,
				childServices
				);
		public static IServiceInstanceInitializer<I> Service<I, T>(
			this IServiceInstanceManager manager,
			object cfg,
			params IServiceInstanceInitializer[] childServices
			)
			=>
			manager.CreateService<I, T>(
				parent => manager.TryGetService<I, T>(parent),
				(parent, rcfg) => manager.TryAddService<I, T>(parent, rcfg,State: Data.LogicObjectState.Disabled),
				cfg,
				childServices
				);

		static IServiceInstanceInitializer<I> CreateService<I, T>(
			this IServiceInstanceManager manager,
			Func<long?, Task<long>> FindService,
			Func<long?, object, Task<Models.ServiceInstanceEditable>> ServiceCreator,
			object cfg,
			IServiceInstanceInitializer[] childServices
			)
		{
			return CreateBuilder<I,T>(
				typeof(T).Comment().Name,
				async (sp, parent) =>
				{

					var svcId = await FindService(parent);
					if(svcId==0)
						svcId = (await ServiceCreator(parent, new { })).Id;

					var children = new HashSet<IServiceInstanceInitializer>(childServices);
					foreach (var chd in children)
						await chd.Ensure(sp, svcId);

					var rcfg = await ConfigResolve(cfg, sp, svcId, children,0);
					var nsvcId = (await ServiceCreator(parent, cfg)).Id;
					if (nsvcId != svcId)
						throw new InvalidOperationException($"服务初始化{typeof(T)}@{typeof(I)}返回ID不一致：第一次：{svcId},第二次：{nsvcId}");

					await manager.SetEntityState(svcId, Data.LogicObjectState.Enabled);
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
			)=>
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
			) => sc.InitDefaultService<I, T>(null, Config,ParentId);

		public static async Task TestService<I>(
			this IServiceProvider ServiceProvider,
			Func<IServiceInstanceManager, IServiceInstanceInitializer<I>> newInitializer,
			Func<I, Task> Action,
			Func<IServiceProvider, IServiceInstanceManager, long,Task> SetupService=null,
			long? ParentId = null
			) where I:class
		{
			var sim = ServiceProvider.Resolve<IServiceInstanceManager>();
			var initializer = newInitializer(sim);
			var sid = await initializer.Ensure(ServiceProvider, ParentId);
			await SetupService(ServiceProvider, sim, sid);
			var svc=ServiceProvider.Resolve<I>(sid);
			await Action(svc);
			await sim.RemoveAsync(sid);
		}
	}

}
