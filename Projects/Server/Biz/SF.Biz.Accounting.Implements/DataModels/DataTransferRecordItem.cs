using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SF.Sys.Data;

namespace SF.Biz.Accounting.DataModels
{

    /*
	 * 0级
	 *		卖家销售记录
	 *		买家消费明细
	 * 1级
	 *		卖家代销支出
	 *		1级代销收入
	 * 2级 
	 *		2级代销收入
	 */
    //1卖家

    public class DataTransferRecordItem
	{
		[Key]
		public long Id { get; set; }

		public int RecordId { get; set; }

		[ForeignKey(nameof(RecordId))]
		public DataTransferRecord Record { get; set; }

		//ParentId == null   直接销售
		//ParentId != null   代销提成
		public int? ParentId { get; set; }

		[ForeignKey(nameof(ParentId))]
		public DataTransferRecord Parent { get; set; }

		[InverseProperty(nameof(Parent))]
		public ICollection<DataTransferRecord> Children { get; set; }

		//顶级为 0
		public int Level { get; set; }


		[Index("outbound", Order = 1)]
		[Index("inbound", Order = 1)]
		public int AccountTitleId { get; set; }

		[Index("outbound", Order = 2)]
		public long SrcId { get; set; }

		[Index("inbound", Order = 2)]
		public long DstId { get; set; }

		[Index("outbound", Order = 3)]
		[Index("inbound", Order = 3)]
		public DateTime Time { get; set; }


		[ForeignKey(nameof(AccountTitleId))]
		public DataAccountTitle AccountTitle { get; set; }

		public decimal Amount { get; set; }

		[Required]
		[MaxLength(200)]
		public string Title { get; set; }

		[Required]
		[MaxLength(50)]
		public string BizType { get; set; }

		[Required]
		[MaxLength(100)]
		public string BizIdent { get; set; }
	}
}
