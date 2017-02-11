using System;
using SF.Data.Entity;
using SF.Metadata;
using SF.Auth;
using SF.Data;
using SF.Core.ManagedServices.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SF.Core.ManagedServices.Admin
{
	[Comment("默认服务定义管理")]
	public class ServiceDeclarationManager :
		IServiceDeclarationManager
	{
		public Dictionary<string,ServiceDeclaration> Items { get; }
		public ServiceDeclarationManager(Runtime.IServiceMetadata ServiceMetadata)
		{
			this.Items = (from type in ServiceMetadata.ManagedServices.Keys
						  let comment = type.GetCustomAttribute<CommentAttribute>()
						  select new ServiceDeclaration
						  {
							  Id = type.FullName,
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

	}
}
