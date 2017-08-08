using SF.Auth.Identities.Models;
using SF.Core.ServiceManagement.Models;
using SF.KB;
using SF.Metadata;
using System.Threading.Tasks;
using SF.Auth;
using SF.Data.Entity;
using System;
namespace SF.Auth.Identities
{
	public class IdentityQueryArgument : IQueryArgument<long>
	{
		public Option<long> Id { get; set; }
		public string Ident { get; set; }
		public string Name { get; set; }
	}

	[EntityManager("身份标识")]
	[Authorize("admin")]
	[NetworkService]
	[Comment("身份标识")]
	public interface IIdentityManagementService :
		IEntityManager<long, Models.IdentityInternal>,
		IEntitySource<long, Models.IdentityInternal, IdentityQueryArgument>
	{
	}

}

