using SF.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Auth.Permissions
{
	public interface IOperationManager :
        IEntitySource<string,Models.OperationInternal,ObjectQueryArgument<string>>
    {
        
    }

}
