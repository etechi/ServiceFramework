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

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
	/// 默认服务实现管理
	/// </summary>
	public class ServiceImplementManager :
		IServiceImplementManager
	{
		public Dictionary<string, ServiceImplement> Items { get; }

		public long? ServiceInstanceId => null;

		public ServiceImplementManager(IServiceMetadata ServiceMetadata)
		{
			var sis = from impl in ServiceMetadata.ImplementsById
					  let svcType = impl.Value.ServiceType
					  let implType = impl.Value.ImplementType
					  where implType != null
					  let declComment = svcType.Comment()
					  let implComment = implType.Comment()
					  select new ServiceImplement
					  {
						  Id = impl.Key,
						  Type=implType.GetFullName(),
						  Description = implComment?.Summary,
						  Name = implComment?.Title ?? implType.Name,
						  Group = implComment?.Group,
						  DeclarationId = svcType.GetFullName(),
						  DeclarationName = declComment?.Title ?? svcType.Name
					  };
			this.Items = sis.ToDictionary(t => t.Id);
		}

		public Task<ServiceImplement[]> BatchGetAsync(ObjectKey<string>[] Ids, string[] Properties)
		{
			return Task.FromResult(
				Ids
				.Select(id => Items.Get(id.Id))
				.Where(i => i != null)
				.ToArray()
				);
		}

		public Task<ServiceImplement> GetAsync(ObjectKey<string> Id)
		{
			return Task.FromResult(Items.Get(Id.Id));
		}

		static PagingQueryBuilder<ServiceImplement> pageQueryBuilder = new PagingQueryBuilder<ServiceImplement>(
			"name",
			b => b.Add("name", f => f.Name)
			);
		public Task<QueryResult<ServiceImplement>> QueryAsync(ServiceImplementQueryArgument Arg)
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
					Arg.Paging
					)
					);
		}
		public async Task<QueryResult<ObjectKey<string>>> QueryIdentsAsync(ServiceImplementQueryArgument Arg)
		{
			var re = await QueryAsync(Arg);
			return re.Select(i =>new ObjectKey<string> { Id = i.Id });
		}
	}
}
