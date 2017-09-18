using System.Threading.Tasks;
using SF.Entities;

namespace SF.Auth.Permissions
{
	
	public interface IPermissionService 	{
		IPermission[] GetPermission(long Id);
	}
}
