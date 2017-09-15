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

		public ServiceFactory(
			long Id,
			long? ParentServiceId,
			IServiceDeclaration ServiceDeclaration,
			IServiceImplement ServiceImplement,
			IServiceCreateParameterTemplate CreateParameterTemplate,
			ServiceCreator Creator
			)
		{
			this.InstanceId = Id;
			this.ParentInstanceId = ParentServiceId;
			this.ServiceDeclaration = ServiceDeclaration;
			this.ServiceImplement = ServiceImplement;
			this.CreateParameterTemplate = CreateParameterTemplate;
			this.Creator = Creator;
		}

		public object Create(
			IServiceResolver ServiceResolver
			)
		{
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
				.AssertNotNull(
					() => $"�Ҳ�����������({svcTypeName}),����ID:{Id}"
					)
				.Assert(
					type =>ServiceType==null || type == ServiceType,
					type => $"����ʵ��({Id})�ķ�������({type})��ָ����������({ServiceType})��һ��"
					);

			var decl = ServiceMetadata.Services
				.Get(declType)
				.AssertNotNull(
					() => $"�Ҳ�����������({declType}),����:{Id}"
					);

			var implType = ServiceResolver.Resolve<IServiceImplementTypeResolver>()
				.Resolve(implTypeName)
				.AssertNotNull(
					() => $"�Ҳ�����������({Id})ָ���ķ���ʵ������({implTypeName}),����:{declType}"
					);

			var impl = decl.Implements
				.Last(i => i.ServiceImplementType == ServiceImplementType.Type && i.ImplementType == implType)
				.AssertNotNull(
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
						(Creator, CreateParameterTemplate) =
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
				Creator
				);
		}
	}

}
