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

using IdentityServer4.Services;
using IdentityServer4.Stores;
using SF.Metadata;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Models;
using SF.Data;
using System.Linq;
using SF.Core.ServiceManagement.Management;
using SF.Entities;
using SF.Auth.IdentityServices.Internals;

namespace SF.Auth.IdentityServices.IdentityServer4Impl
{
	public class ResourceStore : IResourceStore
	{
		IDataSet<DataModels.Resource> Resources { get; }
		IServiceInstanceManager ServiceInstanceManager { get; }
		public ResourceStore(
		IServiceInstanceManager ServiceInstanceManager, 
			IDataSet<DataModels.Resource> Resources
			)
		{
			this.Resources = Resources;
			this.ServiceInstanceManager = ServiceInstanceManager;
		}

		ApiResource MapResource(DataModels.Resource res)
			=> new ApiResource(res.Id, res.Name){
				Scopes = res.SupportedOperations
				.Select(s => 
					new Scope(res.Id+ "." + s.OperationId, res.Name+"/"+(s.Operation?.Name ?? s.OperationId))
					{
						Description = s.Operation?.Description
					})
					.ToArray()
				};
		public async Task<ApiResource> FindApiResourceAsync(string name)
		{
			var res = await Resources
				.AsQueryable()
				.Where(r => r.Id == name && r.LogicState==Entities.EntityLogicState.Enabled)
				.Include(r => r.SupportedOperations)
				.SingleOrDefaultAsync();

			if (res == null)
				return null;
			return MapResource(res);
		}

		public async Task<IEnumerable<ApiResource>> FindApiResourcesByScopeAsync(IEnumerable<string> scopeNames)
		{
			var rids = await Resources.Context.Set<DataModels.ResourceSupportedOperation>().AsQueryable()
				.Where(o => scopeNames.Contains(o.ResourceId + "." + o.OperationId))
				.Select(r => r.ResourceId)
				.Distinct()
				.ToArrayAsync();

			if (rids.Length == 0)
				return Enumerable.Empty<ApiResource>();

			var ress = await (
				from r in Resources.AsQueryable()
				//where rids.Contains(r.Id)
				select r
				)
				.Include(r => r.SupportedOperations)
				.ToArrayAsync();
			return ress.Select(MapResource);
		}

		public async Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeAsync(IEnumerable<string> scopeNames)
		{
			var svcs = await ServiceInstanceManager.QueryAsync(new ServiceInstanceQueryArgument
			{
				ServiceId = IServiceInstanceManagerExtension.GetServiceId(typeof(IUserCredentialProvider)),
			}, Paging.All);

			var re = svcs.Items
				.Where(p => scopeNames.Contains(p.ServiceIdent))
				.Select(p => new IdentityResource
				{
					Name = p.ServiceIdent,
					DisplayName = p.Name,
					Description = p.Description,
					Enabled = true
				});
			return re;
		}

		public async Task<Resources> GetAllResourcesAsync()
		{
			var apiRess=await Resources
				.AsQueryable()
				.Where(r=>r.LogicState==Entities.EntityLogicState.Enabled)
				.Include(r=>r.SupportedOperations)
				.ToArrayAsync();

			var idsvcs = await ServiceInstanceManager.QueryAsync(new ServiceInstanceQueryArgument
			{
				ServiceId = IServiceInstanceManagerExtension.GetServiceId(typeof(IUserCredentialProvider)),
			}, Paging.All);


			return new Resources(
				idsvcs.Items.Select(p => new IdentityResource
				{
					Name = p.ServiceIdent,
					DisplayName = p.Name,
					Description = p.Description,
					Enabled = true
				}),
				apiRess.Select(MapResource)
				);
		}
	}
}
