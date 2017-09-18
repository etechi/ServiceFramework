using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Auth
{
	public interface IPermissionProvider
	{
		Task<IPermission[]> GetPermissions(long OperatorId);
	}
}
