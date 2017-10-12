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
using SF.Core.ServiceManagement;
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
	partial class ServiceActionDescriptor : HttpActionDescriptor
	{
		public class ParameterDescriptor : ReflectedHttpParameterDescriptor
		{
			bool FromBody { get; }
			public ParameterDescriptor(
				HttpActionDescriptor actionDescriptor, 
				ParameterInfo parameterInfo,
				bool FromBody
				) : 
				base(actionDescriptor, parameterInfo)
			{
				this.FromBody = FromBody;
			}
			public override Collection<TAttribute> GetCustomAttributes<TAttribute>()
			{
				var re=base.GetCustomAttributes<TAttribute>();
				if (typeof(TAttribute) == typeof(FromBodyAttribute) && re.Count == 0 && FromBody)
						return new Collection<TAttribute>(
							new[] { (TAttribute)(object)new FromBodyAttribute() }
							);
				return re;
			}
		}
	}
	
}
