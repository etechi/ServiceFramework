using SF.Entities;
using SF.Metadata;
using System;
namespace SF.Auth.Identities
{
	public class IdentityQueryArgument : IQueryArgument<ObjectKey<long>>
	{
		public ObjectKey<long> Id { get; set; }
		public string Ident { get; set; }
		public string Name { get; set; }
	}

	[EntityManager("身份标识")]
	[Authorize("admin")]
	[NetworkService]
	[Comment("身份标识")]
	public interface IIdentityManagementService :
		IEntityManager<ObjectKey<long>,Models.IdentityEditable>,
		IEntitySource<ObjectKey<long>, Models.IdentityInternal, IdentityQueryArgument>
	{
	}

}

