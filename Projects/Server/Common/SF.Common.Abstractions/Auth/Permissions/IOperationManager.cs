using SF.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Auth.Permissions
{
	public interface IOperationManager :
        IEntitySource<ObjectKey<string>, Models.OperationInternal,ObjectQueryArgument<ObjectKey<string>>>
    {
        
    }

}
