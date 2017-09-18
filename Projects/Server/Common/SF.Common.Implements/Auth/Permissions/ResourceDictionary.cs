using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SF.Auth.Permissions.Models;
using SF.Entities;
namespace SF.Auth.Permissions
{
	public class ResourceDictionary:
		Dictionary<string, ResourceInternal>,
		IReadOnlyDictionary<string, ResourceInternal>
	{
		public ResourceDictionary(IEnumerable<IResource> items, IReadOnlyDictionary<string, OperationInternal> Operations)
		{
			foreach (var it in items)
				Add(it.Id, new ResourceInternal
				{
					Id = it.Id,
					CreatedTime = DateTime.Now,
					LogicState = EntityLogicState.Enabled,
					Name = it.Name,
					UpdatedTime = DateTime.Now,
					Group = it.Group,
					AvailableOperations = it.AvailableOperations.Select(o =>
						Operations.Get(o)??throw new ArgumentException($"构造资源时，发现未定义的操作:{o}")
					).ToArray()
				});
		}
	}
}
