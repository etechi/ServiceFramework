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

using IdentityServer4.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Models;
using System.Linq;
using System.Security.Claims;
using SF.Sys.Data;
using SF.Sys.Auth;
using SF.Sys.Entities;
using SF.Sys;
using SF.Sys.Linq;

namespace SF.Auth.IdentityServices.IdentityServer4Impl
{
	public class ProfileService : IProfileService
	{
		IUserProfileService UserProfileService { get; }
		public ProfileService(IUserProfileService UserProfileService)
		{
			this.UserProfileService = UserProfileService;
		}
		public async Task GetProfileDataAsync(ProfileDataRequestContext context)
		{
			var id = context.Subject.GetUserIdent();
			if (!id.HasValue)
				return;

			var cs = await UserProfileService.GetClaims(
				id.Value,
				context.RequestedClaimTypes.ToArray(),
				context.Client.Claims
				);
			context.IssuedClaims = cs.ToList();
		}
		public async Task IsActiveAsync(IsActiveContext context)
		{
			var id=context.Subject.GetUserIdent();
			if (!id.HasValue)
			{
				context.IsActive = false;
				return;
			}
			context.IsActive = await UserProfileService.IsValid(id.Value);
		}
	}
}

