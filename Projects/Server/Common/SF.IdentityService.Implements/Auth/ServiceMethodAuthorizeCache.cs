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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using SF.Auth.IdentityServices.Internals;
using SF.Sys.Auth;
using SF.Sys.Data;
using SF.Sys.Entities;
using SF.Sys.Events;
using SF.Sys.Linq;
using SF.Sys.NetworkService;
using SF.Sys.Reflection;
using SF.Sys.Services;

namespace SF.Auth.IdentityServices
{
	public enum ServiceMethodAuthorizeType
	{
		Anonymouse,
		User,
		UserWithRoles,
		SuperAdmin
	}
	public class ServiceMethodAuthorizeCache
	{
		public Dictionary<(string,string), ServiceMethodAuthorizeType> Methods { get; }
		public ServiceMethodAuthorizeCache(IServiceMetadata meta)
		{
			Methods = (
				from s in meta.Services.Values
				where s.ServiceType.IsDefined(typeof(NetworkServiceAttribute), true)
				let svcId = s.ServiceType.GetFullName()
				let aas= s.ServiceType.GetCustomAttributes<DefaultAuthorizeAttribute>(true)
				let svcType = s.ServiceType.IsDefined(typeof(AnonymousAllowedAttribute), true) ? ServiceMethodAuthorizeType.Anonymouse :
						aas.Any(a=>a.RoleIdent==null)? ServiceMethodAuthorizeType.User:
						aas.Any(a => a.RoleIdent != null) ? ServiceMethodAuthorizeType.UserWithRoles :
						ServiceMethodAuthorizeType.SuperAdmin
				from relType in s.ServiceType.AllRelatedTypes()
				from method in relType.AllPublicInstanceMethods()
				let maas = method.GetCustomAttributes<DefaultAuthorizeAttribute>(true)
				let methodType = method.IsDefined(typeof(AnonymousAllowedAttribute), true) ? ServiceMethodAuthorizeType.Anonymouse :
						maas.Any(a => a.RoleIdent == null) ? ServiceMethodAuthorizeType.User :
						maas.Any(a => a.RoleIdent != null) ? ServiceMethodAuthorizeType.UserWithRoles :
						ServiceMethodAuthorizeType.SuperAdmin
				let type = methodType == ServiceMethodAuthorizeType.SuperAdmin ? svcType : methodType
				where type != ServiceMethodAuthorizeType.SuperAdmin
				select (svcId, method.Name, type)
				).ToDictionary(i => (i.svcId, i.Name), i => i.Item3);
			
		}
		public ServiceMethodAuthorizeType GetAuthorizeType(string Service,string Method)
		{
			return Methods.TryGetValue((Service,Method),out var type)?type: ServiceMethodAuthorizeType.SuperAdmin;
		}
	}
}
