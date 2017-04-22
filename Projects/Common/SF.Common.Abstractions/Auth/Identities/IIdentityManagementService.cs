using SF.Auth.Identities.Models;
using SF.Core.ManagedServices.Models;
using SF.KB;
using SF.Metadata;
using System.Threading.Tasks;
using SF.Auth;
using SF.Data.Entity;

namespace SF.Auth.Identities
{
	public class IdentityQueryArgument : IQueryArgument<long>
	{
		public Option<long> Id { get; set; }
		public string Ident { get; set; }
		public string Name { get; set; }
	}
	
	[NetworkService]
	[Authorize(Roles ="admin")]
	public interface IIdentityManagementService :
		Data.Entity.IEntitySource<long, Models.IdentityInternal, IdentityQueryArgument>
	{
		
	}

}

