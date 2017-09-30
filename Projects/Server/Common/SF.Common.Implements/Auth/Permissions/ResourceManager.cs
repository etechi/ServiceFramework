using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SF.Auth.Permissions.Models;
using SF.Entities;
namespace SF.Auth.Permissions
{
	public class ResourceManager :
		ConstantObjectQueryableEntitySource<ObjectKey<string>, Models.ResourceInternal>,
        IResourceManager
    {
		public ResourceManager(IEntityManager EntityManager, IReadOnlyDictionary<ObjectKey<string>, ResourceInternal> Models) : base(EntityManager, Models)
		{
		}

		public Task<OperationInternal[]> GetResourceOperations(string Id)
        {
            var re = Models.Get(ObjectKey.From(Id));
            if (re == null)
                return Task.FromResult(Array.Empty<OperationInternal>());
            return Task.FromResult(re.AvailableOperations.Select(o => new OperationInternal
            {
                Id = o.Id,
                CreatedTime = o.CreatedTime,
				LogicState=o.LogicState,
				UpdatedTime=o.UpdatedTime,
                Name = o.Name
            }).ToArray());
        }

	}
}
