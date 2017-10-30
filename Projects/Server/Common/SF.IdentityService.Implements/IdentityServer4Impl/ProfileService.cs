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
using SF.Metadata;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Models;
using SF.Data;
using System.Linq;
using System.Security.Claims;

namespace SF.Auth.IdentityServices.IdentityServer4Impl
{
	public class ProfileService : IProfileService
	{
		IDataSet<DataModels.User> Users { get; }
		public ProfileService(IDataSet<DataModels.User> Users)
		{
			this.Users = Users;
		}
		public async Task GetProfileDataAsync(ProfileDataRequestContext context)
		{
			var id = context.Subject.GetUserIdent();
			if (!id.HasValue)
				return;
			var desc = await Users.AsQueryable()
				.Where(u => u.Id == id.Value && u.ObjectState==Entities.EntityLogicState.Enabled)
				.Select(u => new {
					name = u.Name,
					icon = u.Icon
				})
				.SingleOrDefaultAsync();
			if (desc == null)
				return;
			context.AddRequestedClaims(new[]
			{
				new Claim("name",desc.name),
				new Claim("icon",desc.icon)
			});
		}

		public async Task IsActiveAsync(IsActiveContext context)
		{
			var id=context.Subject.GetUserIdent();
			if (!id.HasValue)
			{
				context.IsActive = false;
				return;
			}
			var state= await Users.AsQueryable()
				.Where(u => u.Id == id.Value)
				.Select(u => new { state = u.ObjectState })
				.SingleOrDefaultAsync();
			context.IsActive = state?.state == Entities.EntityLogicState.Enabled;
		}
	}
}

