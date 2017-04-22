using SF.Data.Storage;
using SF.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SF.Metadata;

namespace SF.Auth.Identities.DataModels
{
	[Table("SysAuthIdentityCredential")]
	public class IdentityCredential
	{

		[Key]
		[Column(Order = 1)]
		[Comment("分区ID")]
		[Index("union", Order = 1)]
		public long ScopeId { get; set; }

		[Key]
		[Column(Order =2)]
		[Index("union",Order=2)]
		[MaxLength(50)]
		public string Provider { get; set; }

		[Key]
		[Column(Order = 3)]
		[MaxLength(100)]
		public string Credential { get; set; }

		[Index]
		[Comment("应用ID")]
		public long AppId { get; set; }

		[Index]
		[Comment("标识ID")]
		public long IdentityId { get; set; }

		[Index("union", Order = 3)]
		public string UnionIdent { get; set; }

		public DateTime CreatedTime { get; set; }
		public DateTime? ConfirmedTime { get; set; }

		[ForeignKey(nameof(IdentityId))]
		public Identity Identity { get; set; }

	}

}
