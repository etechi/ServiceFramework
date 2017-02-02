using SF.Core.DI;
using SF.Metadata;
using SF.Services.Management;
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
	class ServiceController : ApiController
	{
		public Type ControllerType { get; }
		public object ControllerInstance { get; }
		public ServiceController(Type ControllerType, object ControllerInstance)
		{
			this.ControllerType = ControllerType;
			this.ControllerInstance = ControllerInstance;
		}
		public override Task<HttpResponseMessage> ExecuteAsync(HttpControllerContext controllerContext, CancellationToken cancellationToken)
		{
			return base.ExecuteAsync(controllerContext, cancellationToken);
		}
	}
	
}
