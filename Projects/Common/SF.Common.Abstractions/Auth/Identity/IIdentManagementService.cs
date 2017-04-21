using SF.Auth.Identity.Models;
using SF.Core.ManagedServices.Models;
using SF.KB;
using SF.Metadata;
using System.Threading.Tasks;
using SF.Auth;
using SF.Data.Entity;

namespace SF.Auth.Identity
{
	public class IdentQueryArgument : IQueryArgument<long>
	{
		public Option<long> Id { get; set; }
		public string Ident { get; set; }
	}
	
	public interface IIdentManagementService :
		Data.Entity.IEntitySource<long, Models.IdentInternal, IdentQueryArgument>
	{
		
	}

}

