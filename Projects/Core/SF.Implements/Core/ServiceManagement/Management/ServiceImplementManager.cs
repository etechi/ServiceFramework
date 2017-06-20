using SF.Auth;
using SF.Data;
using SF.Data.Entity;
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
			this.Items = (
				from svc in ServiceMetadata.Services
				from impl in svc.Value.Implements
				let svcType=svc.Value.ServiceType
				let implType= impl.Interfaces.FirstOrDefault(i=>i.Type==svc.Value.ServiceType)?.ImplementType
				let declComment= svcType.GetCustomAttribute<CommentAttribute>()
				let implComment = implType.GetCustomAttribute<CommentAttribute>()
				select new ServiceImplement
				{
					Id = implType.FullName+'@'+svcType.FullName,
					Description = implComment?.Description,
					Name = implComment?.Name ?? implType.Name,
					Group = implComment?.GroupName,
					DeclarationId=svcType.FullName,
					DeclarationName=declComment?.Name ?? svcType.Name
				}).ToDictionary(t => t.Id);
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
			if (Arg.Id.HasValue)
				q = q.Where(i => i.Id == Arg.Id.Value);
			else
				q = q.Filter(Arg.Name, i => i.Name)
					.Filter(Arg.CategoryId, i => i.Group)
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

	}
}
