using System;
using System.Threading.Tasks;
using System.Linq;
using System.Reflection;
using SF.Data.Entity;
using System.Collections.Generic;

namespace SF.Core.ServiceManagement.Management
{

	public static class ServiceInstanceInitializerImplement
	{

		class ServiceBuilder<T> : IServiceInstanceInitializer<T>
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
				_ServiceId = await Builder(ServiceProvider, ParentId);
				return _ServiceId;
			}
		}
		static IServiceInstanceInitializer<T> CreateBuilder<T>(string Name, Func<IServiceProvider, long?, Task<long>> builder)
		{
			return new ServiceBuilder<T>(Name, builder);
		}

		static async Task<object> ConfigResolve(object config, IServiceProvider ServiceProvider, long? ParentId, HashSet<IServiceInstanceInitializer> Children)
		{
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
					re[i] = await ConfigResolve(arr.GetValue(i), ServiceProvider, ParentId, Children);
				return re;
			}
			if (type is IServiceInstanceInitializer)
			{
				var sb = (IServiceInstanceInitializer)config;
				var sid = await sb.Ensure(ServiceProvider, ParentId);
				return sid;
			}
			var props = config.GetType().GetProperties(
				BindingFlags.Public |
				BindingFlags.Instance |
				BindingFlags.GetProperty |
				BindingFlags.FlattenHierarchy
				);
			var dic = new Dictionary<string, object>();
			foreach (var prop in props)
				dic[prop.Name] = await ConfigResolve(prop.GetValue(config), ServiceProvider, ParentId, Children);
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
				(parent, rcfg) => manager.EnsureDefaultService<I, T>(parent, rcfg),
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
				(parent, rcfg) => manager.TryAddService<I, T>(parent, rcfg),
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
			return CreateBuilder<I>(
				typeof(T).Comment().Name,
				async (sp, parent) =>
				{
					var svcId = await FindService(parent);

					//var fpr = await sim.ResolveDefaultService<IFilePathResolver>();
					//var fc = await sim.ResolveDefaultService<IFileCache>();
					var children = new HashSet<IServiceInstanceInitializer>(childServices);
					var rcfg = ConfigResolve(cfg, sp, svcId == 0 ? (long?)null : svcId, children);
					var ms = await ServiceCreator(parent, rcfg);

					foreach (var chd in children)
					{
						var cid = await chd.Ensure(sp, ms.Id);
						await manager.SetServiceParent(cid, ms.Id);
					}
					return ms.Id;
				});
		}
		public static IServiceCollection InitServices(
			this IServiceCollection sc,
			string Name,
			Func<IServiceProvider,IServiceInstanceManager,Task> initializer
			)
		{
			sc.AddInitializer("service",Name, (sp) =>
			{
				Task.Run(async () =>
				{
					var sim = sp.Resolve<IServiceInstanceManager>();
					await initializer(sp, sim);
				}).Wait();
			});
			return sc;
		}
		public static IServiceCollection InitService(
			this IServiceCollection sc,
			string Name,
			Func<IServiceProvider, IServiceInstanceManager, IServiceInstanceInitializer> initializer
			)
		{
			return sc.InitServices(
				Name,
				async (sp, sim) =>
				{
					var sii = initializer(sp, sim);
					await sii.Ensure(sp, null);
				});
		}
		public static IServiceCollection InitDefaultService<I,T>(
			this IServiceCollection sc,
			string Name,
			object Config
			)=>
			sc.InitService(
				Name ?? "³õÊ¼»¯"+typeof(T).Comment().Name,
				(sp, sim) =>sim.DefaultService<I,T>(Config)
				);
		public static IServiceCollection InitDefaultService<I, T>(
			this IServiceCollection sc,
			object Config
			) => sc.InitDefaultService<I, T>(null, Config);
	}

}
