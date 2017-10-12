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

using SF.Auth;
using SF.Data;
using SF.Entities;
using SF.Metadata;
using SF.Core.ServiceManagement.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
namespace SF.Core.ServiceManagement.Management
{
	[Comment("Ĭ�Ϸ���ʵ�ֹ���")]
	public class ServiceImplementManager :
		IServiceImplementManager
	{
		public Dictionary<string, ServiceImplement> Items { get; }
		public ServiceImplementManager(IServiceMetadata ServiceMetadata)
		{
			var sis = from impl in ServiceMetadata.ImplementsById
					  let svcType = impl.Value.ServiceType
					  let implType = impl.Value.ImplementType
					  where implType != null
					  let declComment = svcType.GetCustomAttribute<CommentAttribute>()
					  let implComment = implType.GetCustomAttribute<CommentAttribute>()
					  select new ServiceImplement
					  {
						  Id = impl.Key,
						  Type=implType.GetFullName(),
						  Description = implComment?.Description,
						  Name = implComment?.Name ?? implType.Name,
						  Group = implComment?.GroupName,
						  DeclarationId = svcType.GetFullName(),
						  DeclarationName = declComment?.Name ?? svcType.Name
					  };
			this.Items = sis.ToDictionary(t => t.Id);
		}

		public Task<ServiceImplement[]> GetAsync(string[] Ids)
		{
			return Task.FromResult(
				Ids
				.Select(id => Items.Get(id))
				.Where(i => i != null)
				.ToArray()
				);
		}

		public Task<ServiceImplement> GetAsync(string Id)
		{
			return Task.FromResult(Items.Get(Id));
		}

		static PagingQueryBuilder<ServiceImplement> pageQueryBuilder = new PagingQueryBuilder<ServiceImplement>(
			"name",
			b => b.Add("name", f => f.Name)
			);
		public Task<QueryResult<ServiceImplement>> QueryAsync(ServiceImplementQueryArgument Arg, Paging paging)
		{
			var q = Items.Values.AsContextQueryable();
			if (Arg.Id!=null)
				q = q.Where(i => i.Id == Arg.Id.Id);
			else
				q = q.Filter(Arg.Name, i => i.Name)
					.Filter(Arg.Group, i => i.Group)
					.Filter(Arg.DeclarationId,i=>i.DeclarationId)
					;
			return Task.FromResult(
				q.ToQueryResult(
					qi => qi,
					r => r,
					pageQueryBuilder,
					paging
					)
					);
		}
		public async Task<QueryResult<string>> QueryIdentsAsync(ServiceImplementQueryArgument Arg, Paging paging)
		{
			var re = await QueryAsync(Arg, paging);
			return re.Select(i => i.Id);
		}
	}
}
