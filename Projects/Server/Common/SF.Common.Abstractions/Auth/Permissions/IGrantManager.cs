using System.Threading.Tasks;
using SF.Entities;
using SF.Metadata;
using SF.Auth.Identities;
namespace SF.Auth.Permissions
{
	public class GrantQueryArgument : ObjectQueryArgument<long>
	{
		[EntityIdent(typeof(SF.Auth.Identities.IIdentityManagementService))]
		public long? IdentityId { get; set; }
	}
	public interface IGrantManager<TGrantEditable>
        : IEntitySource<long, TGrantEditable, GrantQueryArgument>,
		IEntityManager<long,TGrantEditable>
		where TGrantEditable : Models.GrantEditable
	{
	}
	public interface IGrantManager : IGrantManager<Models.GrantEditable>
	{ }
}
