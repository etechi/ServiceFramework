﻿#region Apache License Version 2.0
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
		public IScoped<IDataScope> DataScope { get; }
		public UserProfileService(IScoped<IDataScope> DataScope)
		{
			this.DataScope= DataScope;
		}
		//public Task<Claim[]> GetClaims(long UserId, string ClientId, string[] Scopes)
		//{
		//	return DataScope.Use(
		//		"获取凭证", 
		//		async ctx =>
		//		{
		//			var q = from u in ctx.Queryable<DataModels.DataUser>()
		//					where u.Id == UserId
		//					from role in u.Roles
		//					from g in role.Role.Grants
		//					select new { g.ResourceId, g.OperationId };
		//			var pairs = await q.ToArrayAsync();
		//			return pairs
		//				.GroupBy(p => p.ResourceId,p=>p.OperationId)
		//				.Select(p => new Claim(p.Key, "|" + p.Distinct().Join("|") + "|"))
		//				.ToArray();
		//		});
		//}

		public async Task<Claim[]> GetClaims(long id, string[] ClaimTypes,IEnumerable<Claim> ExtraClaims)
		{
			return await DataScope.Use(ds=>ds.Use("获取凭证",async ctx =>
			{
				var Users = ctx.Set<DataModels.DataUser>();
				var desc = await (
					from u in Users.AsQueryable()
					where u.Id == id && u.LogicState == EntityLogicState.Enabled
					select new
					{
						name = u.Name,
						icon = u.Icon,
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
					var Credentials = ctx.Set<DataModels.DataUserCredential>();

					//var re = await (
					//	from u in Users.AsQueryable()
					//	where u.Id == id
					//	select new
					//	{
					var userCredentials = await (
								from uc in ctx.Set<DataModels.DataUserCredential>().AsQueryable()
								where uc.UserId == id && types.Contains(uc.ClaimTypeId)
								select new { type = uc.ClaimTypeId, value = uc.Credential }
								 ).ToArrayAsync();

					var userClaims = await (
							from uc in ctx.Set<DataModels.DataUserClaimValue>().AsQueryable()
							where uc.UserId == id && types.Contains(uc.TypeId)
							select new { type = uc.TypeId, value = uc.Value }
								).ToArrayAsync();

					var roleClaims = await (
							from r in ctx.Set<DataModels.DataUserRole>().AsQueryable()
							where r.UserId == id
							from rc in r.Role.ClaimValues
							where types.Contains(rc.TypeId)
							select new { type = rc.TypeId, value = rc.Value }
						).ToArrayAsync();

					//var grants = (await (
					//	from r in ctx.Set<DataModels.DataUserRole>().AsQueryable()
					//	where r.UserId == id
					//	from g in r.Role.Grants
					//	where grantResources.Contains(g.ResourceId)
					//	select new { g.OperationId, g.ResourceId }
					//	).ToListAsync())
					//	.GroupBy(p => p.ResourceId)
					//	.Select(g => (rs: g.Key, os: g.Select(gi=>gi.OperationId).Distinct()));

					var roles = await (
						from r in ctx.Set<DataModels.DataUserRole>().AsQueryable()
						where r.UserId == id
						select r.RoleId
						).ToArrayAsync();
				
					foreach (var c in userCredentials)
						claims.Add(new Claim(c.type, c.value));
					foreach (var c in userClaims)
						claims.Add(new Claim(c.type, c.value));
					foreach (var c in roleClaims)
						claims.Add(new Claim(c.type, c.value));
					//foreach (var g in grants)
					//	claims.Add(new Claim("g:" + g.rs, "|" + g.os.Join("|") + "|"));

					if (types.Contains("role"))
						claims.Add(new Claim("role", roles.Join(" ")));
				}
				return claims.ToArray();
			}));
		}

		public async Task<bool> IsValid(long Id)
		{
			return await DataScope.Use(ds=>ds.Use("检查ID是否有效",async ctx=>
			{
				return await ctx.Set<DataModels.DataUser>().AsQueryable()
					.Where(u => u.Id == Id && u.LogicState == EntityLogicState.Enabled)
					.Select(u => true)
					.SingleOrDefaultAsync();
			}));
		}

		public async Task<User> GetUser(long UserId)
		{
			return await DataScope.Use(ds=>ds.Use("查找用户", async ctx =>
			{
				var user=await (
				from u in ctx.Set<DataModels.DataUser>().AsQueryable()
				where u.Id == UserId && u.LogicState == EntityLogicState.Enabled
				select new User
				{
					Id = u.Id,
					Icon = u.Icon,
					Name = u.Name,
					Roles= u.Roles.Select(r=>r.RoleId).ToArray()
				}
				)
				.SingleOrDefaultAsync();
				return user;
			}
			));
		}
        public async Task<User[]> GetUsers(long[] UserIds)
        {
            return await DataScope.Use(ds => ds.Use("查找用户", async ctx =>
            {
                var users = await (
                    from u in ctx.Set<DataModels.DataUser>().AsQueryable()
                    where UserIds.Contains(u.Id) && u.LogicState == EntityLogicState.Enabled
                    select new User
                    {
                        Id = u.Id,
                        Icon = u.Icon,
                        Name = u.Name,
                        Roles = u.Roles.Select(r => r.RoleId).ToArray()
                    }
                    )
                    .ToArrayAsync();
                return users;
            }
            ));
        }
    }

}
