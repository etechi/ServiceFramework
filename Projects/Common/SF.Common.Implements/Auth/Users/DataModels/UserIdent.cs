using SF.Data.Storage;
using SF.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SF.Auth.Users.DataModels
{
	[Table("SysUserIdent")]
	public class UserIdent
	{
		[Key]
		[Column(Order =1)]
		[Index("union",Order=1)]
		[MaxLength(50)]
		public string Provider { get; set; }

		[Key]
		[Column(Order = 2)]
		[MaxLength(100)]
		public string Ident { get; set; }

		[Index]
		public long UserId { get; set; }

		[Index("union", Order = 2)]
		public string UnionIdent { get; set; }

		public DateTime BindTime { get; set; }
		public DateTime? ConfirmedTime { get; set; }
	}

}
