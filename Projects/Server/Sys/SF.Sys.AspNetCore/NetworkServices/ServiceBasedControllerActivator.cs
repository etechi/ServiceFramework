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

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Internal;
using SF.Sys.Comments;
using SF.Sys.Services;
using System;

namespace SF.Sys.AspNetCore.NetworkServices
{
	[AuthorizeAttribute]
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

			object svc;
			if (actionContext.RouteData.Values.TryGetValue("service", out var sid) && sid is string)
			{
				if (long.TryParse((string)sid, out var svcId))
				{
					if (svcId > 0)
						svc = services.Resolver().ResolveServiceByIdent(svcId, serviceType);
					else
						svc = services.GetService(serviceType);
				}
				else
					svc = services.Resolver().ResolveServiceByType(null, serviceType, (string)sid);
			}
			else
			{
				var accessor = services.GetService(typeof(Microsoft.AspNetCore.Http.IHttpContextAccessor));
				if (accessor == null)
					throw new Exception("找不到IHttpContextAccessor服务 ");
				svc = services.GetService(serviceType);
			}

			if (svc == null)
			{
				if (serviceType.IsInterface)
					throw new PublicArgumentException("找不到指定服务:"+serviceType.Comment().Title);
				svc = base.Create(actionContext);
				if (svc != null)
					actionContext.HttpContext.Items[IsCreatedFromDefaultActivator] = IsCreatedFromDefaultActivator;
			}
			return svc;
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
