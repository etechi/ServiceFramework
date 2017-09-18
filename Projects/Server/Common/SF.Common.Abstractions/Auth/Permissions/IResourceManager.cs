using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SF.Entities;

namespace SF.Auth.Permissions
{
	public interface IResourceManager :
      IEntitySource<string,Models.ResourceInternal,ObjectQueryArgument<string>>
    {
        Task<Models.OperationInternal[]> GetResourceOperations(string Id);
    }
   
}
