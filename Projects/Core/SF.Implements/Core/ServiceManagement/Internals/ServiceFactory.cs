using System;

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace SF.Core.ServiceManagement.Internals
{
	class ServiceFactory : IServiceFactory
	{
		public long Id { get; }
		public Type ServiceType { get; }

		public IServiceDeclaration ServiceDeclaration { get;  }
		public IServiceImplement ServiceImplement { get; }

		public IServiceCreateParameterTemplate CreateParameterTemplate { get; }
		public ServiceCreator Creator { get; }

		(IServiceImplement,string) LoadImplementById(IServiceResolver ServiceResolver,IServiceMetadata ServiceMetadata,long Id)
		{
			var ImplementTypeResolver =
					   ServiceResolver
					   .Resolve<IServiceImplementTypeResolver>()
					   .AssertNotNull(
						   () => $"�Ҳ�������{typeof(IServiceImplementTypeResolver)}"
						   );

			var CfgLoader = ServiceResolver
				.Resolve<IServiceConfigLoader>()
				.AssertNotNull(
					() => $"�Ҳ�������{typeof(IServiceConfigLoader)}"
					);


			var cfg = CfgLoader
				.GetConfig(ServiceType.FullName, Id)
				.Assert(
					v => v?.ServiceType == ServiceType.FullName,
					v => $"��������({Id})��������({v?.ServiceType})��ʵ����������({v?.ServiceType})����"
					);
			var setting = cfg.Settings;

			var ImplementType = ImplementTypeResolver
				.Resolve(cfg.ImplementType)
				.AssertNotNull(
					() => $"�Ҳ�����������({Id})ָ���ķ���ʵ������({cfg.ServiceType}),����:{ServiceType}"
					);

			var impl= ServiceMetadata.Services
				.Get(ServiceType)?.Implements
				.SingleOrDefault(i=>i.ImplementType==ImplementType)
				.AssertNotNull(
					() => $"�Ҳ�����������({Id})ָ���ķ���ʵ������({ImplementType}),����:{ServiceType}"
					);
			return (impl, setting);
		}
		public ServiceFactory(
			Type ServiceType,
			IServiceDeclaration ServiceDeclaration,
			long Id,
			IServiceResolver ServiceResolver,
			IServiceMetadata ServiceMetadata
			)
		{
			this.Id = Id;
			this.ServiceType = ServiceType;

			this.ServiceDeclaration = ServiceDeclaration;

			string setting = null;
			if (Id == 0)
				ServiceImplement = ServiceDeclaration.Implements.Last();
			else if(Id<0)
			{
				ServiceImplement = ServiceDeclaration.Implements[(int)(ServiceDeclaration.Implements.Count + Id)];
			}
			else
				(ServiceImplement,setting)= LoadImplementById(ServiceResolver, ServiceMetadata, Id);

			switch (ServiceImplement.ServiceImplementType)
			{
				case ServiceImplementType.Creator:
					var func = ServiceImplement.ImplementCreator;
					Creator = (sp, ctr) => func(sp);
					break;
				case ServiceImplementType.Instance:
					var ins = ServiceImplement.ImplementInstance;
					Creator = (sp, ctr) => ins;
					break;
				case ServiceImplementType.Type:

					var implType = ServiceImplement.ImplementType.IsGenericTypeDefinition ?
						ServiceImplement.ImplementType.MakeGenericType(ServiceType.GetGenericArguments()) :
						ServiceImplement.ImplementType;
					var ci = Internals.ServiceCreatorBuilder
						.FindBestConstructorInfo(implType)
						.AssertNotNull(
							() => $"�Ҳ�������ʵ������{implType}�Ĺ��캯��"
							);
					CreateParameterTemplate = ServiceCreateParameterTemplate.Load(
						ci,
						Id,
						setting,
						ServiceMetadata
						);


					Creator = ServiceCreatorBuilder.Build(
						ServiceMetadata,
						implType,
						ci
						);
					break;
			}
		}

		public object Create(
			IServiceResolver ServiceResolver
			)
		{
			return Creator(ServiceResolver, CreateParameterTemplate);
		}
	}

}
