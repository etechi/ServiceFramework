using System;

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace SF.Core.ServiceManagement.Internals
{
	class ServiceFactory : IServiceFactory
	{
		public long? InstanceId { get; }
		public long? ParentInstanceId { get; }
		public IServiceDeclaration ServiceDeclaration { get; }
		public IServiceImplement ServiceImplement { get; }

		public IServiceCreateParameterTemplate CreateParameterTemplate { get; }
		public ServiceCreator Creator { get; }

		public ServiceFactory(
			long? Id,
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
	}

}
