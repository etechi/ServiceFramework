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
using Microsoft.AspNetCore.Mvc.Filters;
using SF.Sys.AspNetCore.Auth;
using SF.Sys.Auth;
using SF.Sys.Clients;
using SF.Sys.Services;
using SF.Sys.TimeServices;
using System.Threading.Tasks;

namespace SF.Sys.AspNetCore.Auth
{
	public class TokenAuthorizationFilter :
		IAuthorizationFilter, 
		IAsyncAuthorizationFilter
	{

		public TokenAuthorizationFilter()
		{
		}

		public void OnAuthorization(AuthorizationFilterContext context)
		{
			var cad = context.ActionDescriptor as ControllerActionDescriptor;
			if (cad == null)
				return ;
			var type = cad.ControllerTypeInfo;
			var method = cad.MethodInfo;
			var user = context.HttpContext.User;
			var sp = context.HttpContext.RequestServices;
			//var client = sp.Resolve<IClientService>();
			var authService = sp.Resolve<IInterfaceAuthService>();
			var re = authService.Authorize(user, type.AsType(), method);
			if (re)
				return;
			context.Result = new ObjectResult(
				new PublicDeniedException("您无权访问此方法")
				);
		}

		public Task OnAuthorizationAsync(AuthorizationFilterContext context)
		{
			OnAuthorization(context);
			return Task.CompletedTask;
		}
	}
}
