using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SF.Sys.Entities.DataModels;
using SF.Sys.Data;
using SF.Sys.Clients;
using System.Collections.Generic;

namespace SF.Biz.Accounting.DataModels
{
    //买家消费记录

    /// <summary>
    /// 转账记录
    /// </summary>
    public class DataTransferRecord:DataEventEntityBase
	{

        /// <summary>
        /// 源科目ID
        /// </summary>
        [Index("outbound", Order = 1)]
        [Index("title",Order =1)]
        public long SrcTitleId { get; set; }

        /// <summary>
        /// 目标科目ID
        /// </summary>
		[Index("inbound", Order = 1)]
        [Index("title", Order = 2)]
        public long DstTitleId { get; set; }

        /// <summary>
        /// 源用户ID
        /// </summary>
		[Index("outbound", Order = 2)]
        public long SrcId { get; set; }

        /// <summary>
        /// 目标用户ID
        /// </summary>
		[Index("inbound", Order = 2)]
        public long DstId { get; set; }

        /// <summary>
        /// 转账时间
        /// </summary>
		[Index("outbound", Order = 3)]
		[Index("inbound", Order = 3)]
        public override DateTime Time { get; set; }


		[ForeignKey(nameof(SrcTitleId))]
		public DataAccountTitle SrcAccountTitle { get; set; }

		[ForeignKey(nameof(DstTitleId))]
		public DataAccountTitle DstAccountTitle { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }
        ///<title>原账户转账后余额</title>
        /// <summary>
        /// 系统虚拟账户总为0
        /// </summary>
        public decimal SrcCurValue { get; set; }

        ///<title>目标账户转账后余额</title>
        /// <summary>
        /// 系统虚拟账户总为0
        /// </summary>
        public decimal DstCurValue { get; set; }

        /// <summary>
        /// 转账标题
        /// </summary>
        [Required]
		[MaxLength(200)]
        public string Title { get; set; }

        /// <summary>
        /// 业务跟踪对象
        /// </summary>
        [MaxLength(100)]
        [Required]
        [Index("biz-ident", IsUnique = true,Order =1)]
        public string TrackEntityIdent { get; set; }

        ///<title>业务记录索引</title>
        /// <summary>
        /// 一个业务有多个转账记录时，以此字段区分
        /// </summary>
        [Index("biz-ident", IsUnique = true, Order = 2)]
        public int BizRecordIndex { get; set; }


        /// <summary>
        /// 操作地址
        /// </summary>
        [MaxLength(100)]
        public string OpAddress { get; set; }

        /// <summary>
        /// 操作设备
        /// </summary>
        public ClientDeviceType OpDevice { get; set; }


        [InverseProperty(nameof(DataTransferRecordItem.Record))]
        public ICollection<DataTransferRecordItem> Items { get; set; }

    }
}
