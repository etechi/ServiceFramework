using SF.Data.Storage;
using SF.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SF.Metadata;

namespace SF.Auth.Identities.Entity.DataModels
{
	
	[Table("SysAuthIdentityCredential")]
	public class IdentityCredential<TIdentity, TIdentityCredential>
		where TIdentity : Identity<TIdentity, TIdentityCredential>
		where TIdentityCredential : IdentityCredential<TIdentity, TIdentityCredential>
	{

		[Key]
		[Column(Order = 1)]
		[Comment("分区ID")]
		[Index("union", Order = 1)]
		public long ScopeId { get; set; }

		[Key]
		[Column(Order =2)]
		[Index("union",Order=2)]
		public long ProviderId { get; set; }

		[Key]
		[Column(Order = 3)]
		[MaxLength(100)]
		public string Credential { get; set; }


		[Index]
		[Comment("标识ID")]
		public long IdentityId { get; set; }

		[Index("union", Order = 3)]
		[MaxLength(100)]
		public string UnionIdent { get; set; }

		public DateTime CreatedTime { get; set; }
		public DateTime? ConfirmedTime { get; set; }

		[ForeignKey(nameof(IdentityId))]
		public TIdentity Identity { get; set; }

	}
	public class IdentityCredential : IdentityCredential<Identity, IdentityCredential>
	{ }
}
