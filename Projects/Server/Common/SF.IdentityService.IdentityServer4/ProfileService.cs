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

			var desc = await (
				from u in Users.AsQueryable()
				where u.Id == id.Value && u.LogicState == EntityLogicState.Enabled
				select new {
					name = u.Name,
					icon = u.Icon,
					phone=u.PhoneNumber,
					image=u.Image
				})
				.SingleOrDefaultAsync();


			var claims = new List<Claim>();


			var grantResources = new HashSet<string>();
			var types = new HashSet<string>(); 
			foreach (var r in context.RequestedClaimTypes)
			{
				var p = r.Split2(':');
				if (p.Item2 == null)
				{
					string value;
					switch (p.Item1)
					{
						case "name":value = desc.name;break;
						case "icon": value = desc.name; break;
						case "image": value = desc.image; break;
						case "phone": value = desc.phone; break;
						case "sub": value = id.ToString(); break;
						default:types.Add(p.Item1);value = null; break;
					}
					if(value!=null)
						claims.Add(new Claim(p.Item1, value));
				}
				else if(p.Item1=="g")
				{
					grantResources.Add(p.Item2);
				}
			}
			foreach (var cc in context.Client.Claims)
				if (types.Contains(cc.Type))
					claims.Add(cc);

			var re = await (
				from u in Users.AsQueryable()
				where u.Id == id.Value && u.LogicState == EntityLogicState.Enabled
				select new {
					userClaims = from uc in u.ClaimValues
									where types.Contains(uc.TypeId)
									select new { type = uc.TypeId, value = uc.Value },
					roleClaims= from r in u.Roles
								from rc in r.Role.ClaimValues
								where types.Contains(rc.TypeId)
								select new { type = rc.TypeId, value = rc.Value },
					grants = from r in u.Roles
								from g in r.Role.Grants
								where grantResources.Contains(g.ResourceId)
								group g.OperationId by g.ResourceId into gg
								select new { rs = gg.Key, os = gg.Distinct() },
					roles = from r in u.Roles
							select r.RoleId
				}
				).SingleOrDefaultAsync();
			if (re != null)
			{
				foreach (var c in re.userClaims)
					claims.Add(new Claim(c.type, c.value));
				foreach (var c in re.roleClaims)
					claims.Add(new Claim(c.type, c.value));
				foreach (var g in re.grants)
					claims.Add(new Claim("g:" + g.rs, "|" + g.os.Join("|") + "|"));

				if (types.Contains("role"))
					claims.Add(new Claim("role", re.roles.Join(" ")));
			}
			context.IssuedClaims = claims;
			
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
				.Select(u => new { state = u.LogicState })
				.SingleOrDefaultAsync();
			context.IsActive = state?.state == EntityLogicState.Enabled;
		}
	}
}

