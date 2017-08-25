using System;
using SF.Entities;
using SF.Metadata;
using SF.Auth;
using SF.Data;
using SF.Core.ServiceManagement.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SF.Core.ServiceManagement.Internals;

namespace SF.Core.ServiceManagement.Management
{
	[Comment("默认服务定义管理")]
	public class ServiceDeclarationManager :
		IServiceDeclarationManager
	{
		public Dictionary<string,ServiceDeclaration> Items { get; }
		public ServiceDeclarationManager(IServiceMetadata ServiceMetadata)
		{
			this.Items = (from svc in ServiceMetadata.Services
						  let type=svc.Key
						  let comment = type.GetCustomAttribute<CommentAttribute>()
						  select new ServiceDeclaration
						  {
							  Id = type.GetFullName(),
							  Description = comment?.Description,
							  Name = comment?.Name ?? type.Name,
							  Group = comment?.GroupName
						  }).ToDictionary(t=>t.Id);
		}

		public Task<ServiceDeclaration[]> GetAsync(string[] Ids)
		{
			return Task.FromResult(
				Ids
				.Select(id => Items.Get(id))
				.Where(i => i != null)
				.ToArray()
				);
		}

		public Task<ServiceDeclaration> GetAsync(string Id)
		{
			return Task.FromResult(Items.Get(Id));
		}

		public Task<QueryResult<ServiceDeclaration>> QueryAsync(ServiceDeclarationQueryArgument Arg, Paging paging)
		{
			var q = Items.Values.AsContextQueryable();
			if (Arg.Id.HasValue)
				q = q.Where(i => i.Id == Arg.Id.Value);
			else
				q = q.Filter(Arg.Name, i => i.Name)
					.Filter(Arg.CategoryId, i => i.Group)
					;
			return Task.FromResult(
				q.ToQueryResult(
					qi => qi,
					r => r,
					PagingQueryBuilder<ServiceDeclaration>.Simple("name",e=>e.Name),
					paging
					)
					);
		}

		public async Task<QueryResult<string>> QueryIdentsAsync(ServiceDeclarationQueryArgument Arg, Paging paging)
		{
			var re = await QueryAsync(Arg, paging);
			return re.Select(i => i.Id);
		}
	}
}
