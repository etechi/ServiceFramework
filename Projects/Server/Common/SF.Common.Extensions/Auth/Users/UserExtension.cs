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
using SF.Auth;
using SF.Auth.Identities;
using SF.Users.Members.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.Auth.Identities.Models;
using SF.Entities;
using SF.Data;
using SF.Core.ServiceManagement;
using SF.Core;

namespace SF.Auth.Users
{
	public static class UserExtension
	{
		public static async Task<TUserInternal> UserEnsure<TUserManagementService,TUserInternal,TUserEditable,TQueryArgument>(
			this IUserService Service,
			IServiceProvider ServiceProvider,
			string name,
			string nick,
			string account,
			string password,
			string[] roles = null,
			Dictionary<string, string> extArgs = null
			) 
			where TUserManagementService : class,Auth.Users.IUserManagementService<TUserInternal,TUserEditable,TQueryArgument>
			where TUserInternal:Auth.Users.Models.UserInternal
			where TUserEditable:Auth.Users.Models.UserEditable
			where TQueryArgument : Auth.Users.UserQueryArgument,new()
		{
			var sid = ((IManagedServiceWithId)Service).ServiceInstanceId;
			var ims = ServiceProvider.Resolve<TUserManagementService>(null, sid);
			var identityService = ServiceProvider.Resolve<IIdentityService>(null, ((IManagedServiceWithId)ims).ServiceInstanceId);

			var member = await ims.QuerySingleAsync(
				new TQueryArgument
				{
					AccountName = account
				});
			long id;
			if (member != null)
			{
				id = member.Id;
			}
			else
			{
				if (roles != null && roles.Length > 0)
				{
					if (extArgs == null)
						extArgs = new Dictionary<string, string>();
					extArgs["roles"] = Json.Stringify(roles);
				}

				var sess = await Service.Signup(
					new CreateIdentityArgument
					{
						Credential = account,
						ExtraArgument = extArgs.Count > 0 ? extArgs : null,
						Identity = new Identity
						{
							Icon = null,
							Name = nick
						},
						Password = password,
						ReturnToken = true,
					}
					);
				var iid = await identityService.ParseAccessToken(sess);
				var ident = await identityService.GetIdentity(iid);
				var mid = ServiceEntityIdent.Parse<long>(ident.OwnerId);
				id = mid.Id2;
			}
			return await ims.GetAsync(ObjectKey.From(id));

		}
	}

}

