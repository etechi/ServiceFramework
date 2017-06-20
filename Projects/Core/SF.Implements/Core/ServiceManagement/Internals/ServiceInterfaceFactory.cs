using System;

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace SF.Core.ServiceManagement.Internals
{
	class ServiceInterfaceFactory : IServiceInterfaceFactory
	{
		public long Id { get; }
		public IServiceDeclaration ServiceDeclaration { get; }
		public IServiceImplement ServiceImplement { get; }
		public IServiceInterface ServiceInterface { get; }

		public IServiceCreateParameterTemplate CreateParameterTemplate { get; }
		public ServiceCreator Creator { get; }

		public ServiceInterfaceFactory(
			long Id,
			IServiceDeclaration ServiceDeclaration,
			IServiceImplement ServiceImplement,
			IServiceInterface ServiceInterface ,
			IServiceCreateParameterTemplate CreateParameterTemplate,
			ServiceCreator Creator
			)
		{
			this.Id = Id;
			this.ServiceDeclaration = ServiceDeclaration;
			this.ServiceImplement = ServiceImplement;
			this.ServiceInterface = ServiceInterface;
			this.CreateParameterTemplate = CreateParameterTemplate;
			this.Creator = Creator;
		}

		public object Create(
			IServiceResolver ServiceResolver
			)
		{
			return Creator(ServiceResolver, CreateParameterTemplate);
		}
	}

}
