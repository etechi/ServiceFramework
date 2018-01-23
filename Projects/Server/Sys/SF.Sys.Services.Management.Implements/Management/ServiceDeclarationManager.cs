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

using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SF.Sys.Services.Management.Models;
using SF.Sys.Comments;
using SF.Sys.Reflection;
using SF.Sys.Entities;
using SF.Sys.Collections.Generic;
using SF.Sys.Linq;

namespace SF.Sys.Services.Management
{
	/// <summary>
	/// 默认服务定义管理
	/// </summary>
	public class ServiceDeclarationManager :
		IServiceDeclarationManager
	{
		public Dictionary<string,ServiceDeclaration> Items { get; }

		public long? ServiceInstanceId => null;

		public ServiceDeclarationManager(IServiceMetadata ServiceMetadata)
		{
			this.Items = (from pair in ServiceMetadata.ServicesById
						  let type=pair.Value.ServiceType
						  let comment = type.Comment()
						  select new ServiceDeclaration
						  {
							  Id = pair.Key,
							  Type=type.GetFullName(),
							  Description = comment?.Summary,
							  Name = type.FriendlyName(),
							  Group = comment?.Group
						  }).ToDictionary(t=>t.Id);
		}

		public Task<ServiceDeclaration[]> BatchGetAsync(ObjectKey<string>[] Ids,string[] Properties)
		{
			return Task.FromResult(
				Ids
				.Select(id => Items.Get(id.Id))
				.Where(i => i != null)
				.ToArray()
				);
		}

		public Task<ServiceDeclaration> GetAsync(ObjectKey<string> Id,string[] Fields=null)
		{
			return Task.FromResult(Items.Get(Id.Id));
		}

		public Task<QueryResult<ServiceDeclaration>> QueryAsync(ServiceDeclarationQueryArgument Arg)
		{
			var q = Items.Values.AsContextQueryable();
			if (Arg.Id!=null)
				q = q.Where(i => i.Id == Arg.Id.Id);
			else
				q = q.Filter(Arg.Name, i => i.Name)
					.Filter(Arg.Group, i => i.Group)
					;
			return Task.FromResult(
				q.ToQueryResult(
					qi => qi,
					r => r,
					PagingQueryBuilder<ServiceDeclaration>.Simple("name",e=>e.Name),
					Arg.Paging
					)
					);
		}

		public async Task<QueryResult<ObjectKey<string>>> QueryIdentsAsync(ServiceDeclarationQueryArgument Arg)
		{
			var re = await QueryAsync(Arg);
			return re.Select(i =>new ObjectKey<string> { Id = i.Id });
		}
	}
}
