using System;

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
namespace SF.Core.ServiceManagement.Internals
{
	class ServiceFactory : IServiceFactory
	{
		public long InstanceId { get; }
		public long? ParentInstanceId { get; }
		public bool IsManaged => InstanceId > 0;
		public IServiceDeclaration ServiceDeclaration { get; }
		public IServiceImplement ServiceImplement { get; }

		public IServiceCreateParameterTemplate CreateParameterTemplate { get; }
		public ServiceCreator Creator { get; }
		public ServiceConfigLoader ConfigLoader { get; }
		public ServiceFactory(
			long Id,
			long? ParentServiceId,
			IServiceDeclaration ServiceDeclaration,
			IServiceImplement ServiceImplement,
			IServiceCreateParameterTemplate CreateParameterTemplate,
			ServiceCreator Creator,
			ServiceConfigLoader ConfigLoader
			)
		{
			this.InstanceId = Id;
			this.ParentInstanceId = ParentServiceId;
			this.ServiceDeclaration = ServiceDeclaration;
			this.ServiceImplement = ServiceImplement;
			this.CreateParameterTemplate = CreateParameterTemplate;
			this.Creator = Creator;
			this.ConfigLoader = ConfigLoader;
		}

		public object Create(
			IServiceResolver ServiceResolver
			)
		{
			if (IsManaged)
				using (ServiceResolver.WithScopeService(InstanceId))
					return Creator(ServiceResolver, this, CreateParameterTemplate);
			return Creator(ServiceResolver, this, CreateParameterTemplate);
		}

		public static (IServiceDeclaration,IServiceImplement) ResolveMetadata(
			IServiceResolver ServiceResolver,
			long Id,
			string svcTypeName,
			string implTypeName,
			Type ServiceType
			)
		{
			var ServiceMetadata = ServiceResolver.Resolve<IServiceMetadata>();

			var declType = ServiceResolver.Resolve<IServiceDeclarationTypeResolver>()
				.Resolve(svcTypeName)
				.IsNotNull(
					() => $"�Ҳ�����������({svcTypeName}),����ID:{Id}"
					)
				.Assert(
					type =>ServiceType==null || type == ServiceType,
					type => $"����ʵ��({Id})�ķ�������({type})��ָ����������({ServiceType})��һ��"
					);

			var decl = ServiceMetadata.Services
				.Get(declType)
				.IsNotNull(
					() => $"�Ҳ�����������({declType}),����:{Id}"
					);

			var implType = ServiceResolver.Resolve<IServiceImplementTypeResolver>()
				.Resolve(implTypeName)
				.IsNotNull(
					() => $"�Ҳ�����������({Id})ָ���ķ���ʵ������({implTypeName}),����:{declType}"
					);

			var impl = decl.Implements
				.Last(i => i.ServiceImplementType == ServiceImplementType.Type && i.ImplementType == implType)
				.IsNotNull(
					() => $"�Ҳ�����������({Id})ָ���ķ���ʵ������({implType}),����:{declType}"
					);
			return (decl, impl);
		}
		public static IServiceFactory Create(
			long Id,
			long? ParentId,
			IServiceDeclaration decl,
			IServiceImplement impl,
			Type ServiceType, 
			ServiceCreatorCache CreatorCache,
			IServiceMetadata ServiceMetadata,
			string Setting
			)
		{
			ServiceCreator Creator;
			ServiceConfigLoader ConfigLoader = (sp, si, ctr) => Array.Empty<KeyValuePair<string, object>>();
			IServiceCreateParameterTemplate CreateParameterTemplate = null;
			switch (impl.ServiceImplementType)
			{
				case ServiceImplementType.Creator:
					var func = impl.ImplementCreator;
					Creator = (sp, si, ctr) => func(sp.Provider);
					break;
				case ServiceImplementType.Instance:
					var ins = impl.ImplementInstance;
					Creator = (sp, si, ctr) => ins;
					break;
				case ServiceImplementType.Method:
					var method = impl.ImplementMethod;
					var fn = method.IsGenericMethodDefinition ?
						method
							.MakeGenericMethod(ServiceType.GetGenericArguments())
							.CreateDelegate<Func<IServiceProvider, object>>() :
						method
						.CreateDelegate<Func<IServiceProvider, object>>();
					Creator = (sp, si, ctr) => fn(sp.Provider);
					break;
				case ServiceImplementType.Type:
					{
						(Creator,ConfigLoader, CreateParameterTemplate) =
							CreatorCache == null ?
							ServiceCreatorCache.CreateServiceInstanceCreator(
								ServiceType,
								impl.ImplementType,
								Setting,
								ServiceMetadata,
								impl.IsManagedService
								) :
							CreatorCache.GetServiceInstanceCreator(
								ServiceType,
								impl.ImplementType,
								Setting,
								impl.IsManagedService
								);
						break;
					}
				default:
					throw new NotSupportedException();
			}

			return new ServiceFactory(
				Id,
				ParentId,
				decl,
				impl,
				CreateParameterTemplate,
				Creator,
				ConfigLoader
				);
		}

		Dictionary<Delegate,Lazy<Action<object>>> _SettingChangedCallbacks;

		public void NotifySettingChanged(object Setting)
		{
			lock (this)
				foreach (var cb in _SettingChangedCallbacks.Values)
					cb.Value(Setting);

		}
		public IDisposable OnSettingChanged<T>(Action<T> Callback)
		{
			if (ServiceImplement.LifeTime != ServiceImplementLifetime.Singleton)
				throw new NotSupportedException("ֻ�е�������֧�����ø���֪ͨ");
			lock (this)
			{
				if (_SettingChangedCallbacks == null)
					_SettingChangedCallbacks = new Dictionary<Delegate, Lazy<Action<object>>>();
				_SettingChangedCallbacks.Add(Callback,new Lazy<Action<object>>(()=>{
					return null;
				}));
				return Disposable.FromAction(() =>
				{
					lock(this)
						_SettingChangedCallbacks.Remove(Callback);
				});
			}

		}
	}

}
