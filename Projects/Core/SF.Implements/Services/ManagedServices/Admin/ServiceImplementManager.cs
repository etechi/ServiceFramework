using SF.Auth;
using SF.Data;
using SF.Data.Entity;
using SF.Metadata;
using SF.Services.ManagedServices.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
namespace SF.Services.ManagedServices.Admin
{
	[Comment("默认服务实现管理")]
	public class ServiceImplementManager :
		IServiceImplementManager
	{
		public Dictionary<string, ServiceImplement> Items { get; }
		public ServiceImplementManager(Runtime.IServiceMetadata ServiceMetadata)
		{
			this.Items = (
				from pair in ServiceMetadata.ManagedServices
				from desc in pair.Value
				let declComment= pair.Key.GetCustomAttribute<CommentAttribute>()
				let implComment = desc.Type.GetCustomAttribute<CommentAttribute>()
				select new ServiceImplement
				{
					Id = desc.Type.FullName+'@'+pair.Key.FullName,
					Description = declComment?.Description,
					Name = declComment?.Name ?? desc.Type.Name,
					Group = declComment?.GroupName,
					DeclarationId=pair.Key.FullName,
					DeclarationName=declComment?.Name ?? pair.Key.Name
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
