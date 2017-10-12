#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0

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
