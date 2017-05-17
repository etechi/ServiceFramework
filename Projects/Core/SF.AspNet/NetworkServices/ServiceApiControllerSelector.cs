using SF.Metadata;
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
using SF.Core.NetworkService;

namespace SF.AspNet.NetworkService
{
	class ServiceApiControllerSelector : DefaultHttpControllerSelector
	{
		Dictionary<string,HttpControllerDescriptor> ServiceControllerDescriptors { get; }
		public ServiceApiControllerSelector(
			HttpConfiguration configuration,
			Type[] ServiceTypes,
			IServiceBuildRuleProvider ServiceBuildRule
			) :base(configuration){
			ServiceControllerDescriptors = 
				ServiceTypes.
				Select(t => new HttpControllerDescriptor(
					configuration, 
					ServiceBuildRule.FormatServiceName(t), 
					t
					))
				.ToDictionary(d => d.ControllerName,StringComparer.CurrentCultureIgnoreCase);
		}
		public override HttpControllerDescriptor SelectController(HttpRequestMessage request)
		{
			var controllerName = GetControllerName(request);
			var desc = ServiceControllerDescriptors.Get(controllerName);
			if (desc != null) return desc;
			return base.SelectController(request);
		}
		public override IDictionary<string, HttpControllerDescriptor> GetControllerMapping()
		{
			return base.GetControllerMapping()
				.Values
				.Union(ServiceControllerDescriptors.Values)
				.ToDictionary(d => d.ControllerName, StringComparer.CurrentCultureIgnoreCase);
		}
	}
	
}
