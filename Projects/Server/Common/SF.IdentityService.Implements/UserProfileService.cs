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

using SF.Auth.IdentityServices.Internals;
using SF.Auth.IdentityServices.Models;
using SF.Sys;
using SF.Sys.Auth;
using SF.Sys.Data;
using SF.Sys.Entities;
using SF.Sys.Linq;
using SF.Sys.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
namespace SF.Auth.IdentityServices
{
	public class UserProfileService : IUserProfileService
	{
		public IScoped<IDataSet<DataModels.User>> UserScope { get; }
		public UserProfileService(IScoped<IDataSet<DataModels.User>> UserScope)
		{
			this.UserScope = UserScope;
		}
		public async Task<Claim[]> GetClaims(long id, string[] ClaimTypes,IEnumerable<Claim> ExtraClaims)
		{
			return await UserScope.Use(async Users =>
			{
				var desc = await (
					from u in Users.AsQueryable()
					where u.Id == id && u.LogicState == EntityLogicState.Enabled
					select new
					{
						name = u.Name,
						icon = u.Icon,
						phone = u.PhoneNumber,
						image = u.Image
					})
					.SingleOrDefaultAsync();
				if (desc == null)
					return Array.Empty<Claim>();

				var claims = new List<Claim>();


				var grantResources = new HashSet<string>();
				var types = new HashSet<string>();
				if(ClaimTypes!=null)
					foreach (var r in ClaimTypes)
					{
						var p = r.Split2(':');
						if (p.Item2 == null)
						{
							string value;
							switch (p.Item1)
							{
								case PredefinedClaimTypes.Name: value = desc.name; break;
								case PredefinedClaimTypes.Icon: value = desc.icon; break;
								case PredefinedClaimTypes.Image: value = desc.image; break;
								case PredefinedClaimTypes.Phone: value = desc.phone; break;
								case PredefinedClaimTypes.Subject: value = id.ToString(); break;
								default: types.Add(p.Item1); value = null; break;
							}
							if (value != null)
								claims.Add(new Claim(p.Item1, value));
						}
						else if (p.Item1 == "g")
						{
							grantResources.Add(p.Item2);
						}
					}
				if (ExtraClaims != null)
					foreach (var cc in ExtraClaims)
						if (types.Contains(cc.Type))
							claims.Add(cc);

				if (types.Count > 0 || grantResources.Count > 0)
				{
					var re = await (
						from u in Users.AsQueryable()
						where u.Id == id
						select new
						{
							userClaims = from uc in u.ClaimValues
										 where types.Contains(uc.TypeId)
										 select new { type = uc.TypeId, value = uc.Value },
							roleClaims = from r in u.Roles
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
				}
				return claims.ToArray();
			});
		}

		public async Task<bool> IsValid(long Id)
		{
			return await UserScope.Use(async Users =>
			{
				return await Users.AsQueryable()
					.Where(u => u.Id == Id && u.LogicState == EntityLogicState.Enabled)
					.Select(u => true)
					.SingleOrDefaultAsync();
			});
		}

		public async Task<User> GetUser(long UserId)
		{
			return await UserScope.Use(async Users =>
			{
				return await Users.AsQueryable()
				.Where(u => u.Id == UserId && u.LogicState == EntityLogicState.Enabled)
				.Select(u => new User
				{
					Id = u.Id,
					Icon = u.Icon,
					Name = u.Name
				})
				.SingleOrDefaultAsync();
			}
			);
		}
	}

}
