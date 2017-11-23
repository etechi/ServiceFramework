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

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Internal;
using SF.Sys.Services;
using System;

namespace SF.Sys.AspNetCore.NetworkServices
{
	public class ServiceBasedControllerActivator : DefaultControllerActivator
	{
		public ServiceBasedControllerActivator(ITypeActivatorCache TypeActivatorCache):base(TypeActivatorCache)
		{
		}
		static object IsCreatedFromDefaultActivator { get; } = new object();
		/// <inheritdoc />
		public override object Create(ControllerContext actionContext)
		{
			if (actionContext == null)
			{
				throw new ArgumentNullException("actionContext");
			}
			Type serviceType = actionContext.ActionDescriptor.ControllerTypeInfo.AsType();
			var services = actionContext.HttpContext.RequestServices;

			long svcId = actionContext.RouteData.Values.TryGetValue("service", out var sid) ? Convert.ToInt64(sid) : 0;

			var s = svcId == 0 ? 
				services.GetService(serviceType) : 
				services.Resolver().ResolveServiceByIdent(svcId, serviceType);
			if (s == null)
			{
				s = base.Create(actionContext);
				if (s != null)
					actionContext.HttpContext.Items[IsCreatedFromDefaultActivator] = IsCreatedFromDefaultActivator;
			}
			return s;
		}

		/// <inheritdoc />
		public override void Release(ControllerContext context, object controller)
		{
			object v;
			if (context.HttpContext.Items.TryGetValue(IsCreatedFromDefaultActivator, out v) && v == IsCreatedFromDefaultActivator)
				base.Release(context, controller);
		}
	}
}
