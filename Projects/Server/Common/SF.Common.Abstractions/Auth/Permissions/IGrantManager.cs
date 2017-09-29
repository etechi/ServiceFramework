using System.Threading.Tasks;
using SF.Entities;
using SF.Metadata;
using SF.Auth.Identities;
using SF.Auth.Identities.Models;

namespace SF.Auth.Permissions
{
	public class GrantQueryArgument : ObjectQueryArgument<long>
	{
		[EntityIdent(typeof(Identity))]
		public long? IdentityId { get; set; }
	}
	public interface IGrantManager<TGrantEditable>
        : IEntitySource< TGrantEditable, GrantQueryArgument>,
		IEntityManager<TGrantEditable>
		where TGrantEditable : Models.GrantEditable
	{
	}
	public interface IGrantManager : IGrantManager<Models.GrantEditable>
	{ }
}
