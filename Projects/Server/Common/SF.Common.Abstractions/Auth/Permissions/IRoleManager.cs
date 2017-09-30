using System.Threading.Tasks;
using SF.Entities;

namespace SF.Auth.Permissions
{
	public class RoleQueryArgument : ObjectQueryArgument<ObjectKey<string>>
	{
	}
	public interface IRoleManager<TRoleInternal,TQueryArgument>
        : IEntityManager<ObjectKey<string>, TRoleInternal>,
		IEntitySource<ObjectKey<string>, TRoleInternal, TQueryArgument>
		where TRoleInternal: Models.RoleInternal
		where TQueryArgument: RoleQueryArgument
	{
	}
	public interface IRoleManager : IRoleManager<Models.RoleInternal, RoleQueryArgument>
	{ }
}
