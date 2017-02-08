using SF.Core.DI;
using SF.Metadata;
using SF.Services.ManagedServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http.Dispatcher;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Linq.Expressions;

namespace SF.AspNet.NetworkService
{

	class ServiceApiControllerActivator : IHttpControllerActivator
	{
		HashSet<Type> ServiceTypes { get; }
		DefaultHttpControllerActivator DefaultActivator { get; } = new DefaultHttpControllerActivator();
		public ServiceApiControllerActivator(Type[] ServiceTypes)
		{
			this.ServiceTypes = new HashSet<Type>(ServiceTypes);
		}

		public IHttpController Create(
			HttpRequestMessage request, 
			HttpControllerDescriptor controllerDescriptor, 
			Type controllerType
			)
		{
			if (ServiceTypes.Contains(controllerType))
				return new ServiceController(controllerType,request.GetDependencyScope().GetService(controllerType));
			return DefaultActivator.Create(request, controllerDescriptor, controllerType);
		}
	}
}
