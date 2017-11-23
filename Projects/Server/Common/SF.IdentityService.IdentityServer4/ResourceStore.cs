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

using IdentityServer4.Stores;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Models;
using System.Linq;
using SF.Sys.Data;
using SF.Sys.Entities;
using SF.Sys.Linq;

namespace SF.Auth.IdentityServices.IdentityServer4Impl
{
	public class ResourceStore : IResourceStore
	{
		IDataContext DataContext { get; }
		public ResourceStore(
			IDataContext DataContext
			)
		{
			this.DataContext = DataContext;
		}
		public async Task<ApiResource> FindApiResourceAsync(string name)
		{
			var re = await (
				from r in DataContext.Set<DataModels.Resource>().AsQueryable()
				where r.Id == name && !r.IsIdentityResource
				where r.LogicState == EntityLogicState.Enabled
				select new
				{
					title = r.Title,
					desceiption = r.Description,
					scopes = from s in r.Scopes
							 let ss=s.Scope
							 where ss.LogicState==EntityLogicState.Enabled
							 select new { id = s.ScopeId, name = ss.Name },
					resClaims=r.RequiredClaims.Select(c=>c.ClaimTypeId)
				} 
				).SingleOrDefaultAsync();
			if (re == null)
				return null;
			return new ApiResource(name, re.title, re.resClaims.WithFirst("g:"+name))
			{
				Description = re.desceiption,
				Scopes = re.scopes.Select(s => new Scope(s.id, s.name)).ToArray()
			};
		}

		public async Task<IEnumerable<ApiResource>> FindApiResourcesByScopeAsync(IEnumerable<string> scopeNames)
		{
			var re = await (
				from s in DataContext.Set<DataModels.Scope>().AsQueryable()
				where scopeNames.Contains(s.Id) && s.LogicState==EntityLogicState.Enabled
				from sr in s.Resources
				let r=sr.Resource
				where r.IsIdentityResource == false && r.LogicState == EntityLogicState.Enabled
				select new
				{
					id = r.Id,
					title = r.Title ,
					desceiption = r.Description ,
					resClaims = r.RequiredClaims.Select(c => c.ClaimTypeId)
				}
				).ToArrayAsync();

			if (re.Length == 0)
				return Enumerable.Empty<ApiResource>();

			return re.Select(r=>new ApiResource(r.id, r.title,r.resClaims.WithFirst("g:"+r.id))
			{
				Description=r.desceiption
			}).ToArray();
		}

		public async Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeAsync(IEnumerable<string> scopeNames)
		{
			var re = await (
				from s in DataContext.Set<DataModels.Scope>().AsQueryable()
				where scopeNames.Contains(s.Id) && s.LogicState == EntityLogicState.Enabled
				from sr in s.Resources
				let r = sr.Resource
				where r.IsIdentityResource == true && r.LogicState == EntityLogicState.Enabled
				select new
				{
					id = r.Id,
					title = r.Title,
					desceiption = r.Description,
					resClaims = r.RequiredClaims.Select(c => c.ClaimTypeId)
				}
				).ToArrayAsync();

			if (re.Length == 0)
				return Enumerable.Empty<IdentityResource>();

			return re.Select(r => new IdentityResource(r.id, r.title, r.resClaims)
			{
				Description = r.desceiption
			}).ToArray();
		}

		public async Task<Resources> GetAllResourcesAsync()
		{
			var ress = await (
				from r in DataContext.Set<DataModels.Resource>().AsQueryable()
				where r.LogicState == EntityLogicState.Enabled
				select new
				{
					id = r.Id,
					isIdentityResource=r.IsIdentityResource,
					title = r.Title ,
					desceiption = r.Description ,
					resClaims = r.RequiredClaims.Select(c => c.ClaimTypeId)
				}
				).ToArrayAsync();

			return new Resources(
				ress.Where(r=>r.isIdentityResource)
				.Select(r => new IdentityResource(r.id,r.title,r.resClaims){
					Description = r.desceiption
				}),
				ress.Where(r => !r.isIdentityResource)
				.Select(r => new ApiResource(r.id, r.title, r.isIdentityResource?r.resClaims:r.resClaims.WithFirst("g:" +r.id))
				{
					Description = r.desceiption
				})
				);
		}
	}
}

