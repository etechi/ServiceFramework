using SF.Data;
using SF.Entities.DataModels;
using SF.Metadata;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SF.Management.BizAdmins.Entity.DataModels
{
	[Table("MgrBizAdmin")]
	public class BizAdmin<TBizAdmin> : ObjectEntityBase
		where TBizAdmin: BizAdmin<TBizAdmin>
	{

		[Comment("账号")]
		[MaxLength(100)]
		[Index]
		public string Account { get; set; }

		[Comment("图标")]
		[MaxLength(100)]
		public string Icon { get; set; }

	}
	public class BizAdmin : BizAdmin<BizAdmin>
	{ }

}

