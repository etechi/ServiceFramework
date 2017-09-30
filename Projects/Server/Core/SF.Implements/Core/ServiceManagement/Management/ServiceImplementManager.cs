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
	[Comment("默认服务实现管理")]
	public class ServiceImplementManager :
		IServiceImplementManager
	{
		public Dictionary<string, ServiceImplement> Items { get; }
		public ServiceImplementManager(IServiceMetadata ServiceMetadata)
		{
			var sis = from svc in ServiceMetadata.Services
					  from impl in svc.Value.Implements
					  let svcType = svc.Value.ServiceType
					  let implType = impl.ImplementType
					  where implType != null
					  let declComment = svcType.GetCustomAttribute<CommentAttribute>()
					  let implComment = implType.GetCustomAttribute<CommentAttribute>()
					  select new ServiceImplement
					  {
						  Id = implType.GetFullName() + '@' + svcType.GetFullName(),
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
