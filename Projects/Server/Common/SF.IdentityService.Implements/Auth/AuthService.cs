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

using System;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using SF.Auth.IdentityServices.Internals;
using SF.Sys.Auth;
using SF.Sys.Entities;

namespace SF.Auth.IdentityServices
{
	public class AuthService : IAuthService, IInterfaceAuthService
	{
		RoleGrantCache RoleGrantCache { get; }
		IEntityMetadataCollection EntityMetadataCollection { get; }
		public AuthService(RoleGrantCache RoleGrantCache, IEntityMetadataCollection EntityMetadataCollection)
		{
			this.RoleGrantCache = RoleGrantCache;
			this.EntityMetadataCollection = EntityMetadataCollection;
		}
		public async Task<AuthResult> Authorize(ClaimsPrincipal User, string Resource, string Operation, object Target)
		{
			var roles = User?.Claims?.FirstOrDefault(c => c.Type == ClaimsIdentity.DefaultRoleClaimType)?.Value?.Split(',');
			if ((roles?.Length ?? 0) == 0)
				return new AuthResult
				{
					Messages = new[] { "您无权访问此操作" },
					Succeeded = false
				};
			var re = await RoleGrantCache.Validate(Resource, Operation, roles);
			if (re)
			{
				return new AuthResult
				{
					Succeeded = true
				};
			}
			else
				return new AuthResult
				{
					Messages = new[] { "您无权访问此操作" },
					Succeeded = false
				};
		}

		public Task<AuthResult> Authorize(ClaimsPrincipal User, Type Service, MethodInfo Method)
		{
			var meta = EntityMetadataCollection.FindByManagerType(Service);
			return null;
		}
	}
}
