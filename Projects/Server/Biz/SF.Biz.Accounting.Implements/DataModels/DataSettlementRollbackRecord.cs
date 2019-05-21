using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SF.Sys.Entities.DataModels;
using SF.Sys.Data;
using SF.Sys.Clients;
using System.Collections.Generic;
using SF.Sys.Annotations;

namespace SF.Biz.Accounting.DataModels
{
    //// <summary>
    /// 结算回退记录
    /// </summary>
    public class DataSettlementRollbackRecord : DataObjectEntityBase
    {
        /// <summary>
        /// 状态
        /// </summary>
        public SettlementRollbackState State { get; set; }

        /// <summary>
        /// 买家
        /// </summary>
        [Index]
        public long BuyerId { get; set; }

        /// <summary>
        /// 卖家
        /// </summary>
        [Index]
        public long SellerId { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [MaxLength(100)]
        public string Desc { get; set; }

        /// <summary>
        /// 结算ID
        /// </summary>
        [Index]
        public long SettlementRecordId { get; set; }

        [ForeignKey(nameof(SettlementRecordId))]
        public DataSettlementRecord SettlementRecord { get; set; }

        /// <summary>
        /// 总回退数额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 退款记录ID
        /// </summary>
        public long? RefundRecordId { get; set; }

        /// <summary>
        /// 退款支付平台Id
        /// </summary>
        public long? RefundPlatformPlatformId { get; set; }

        /// <summary>
        /// 退款科目Id
        /// </summary>
        public long? RefundTitleId { get; set; }

        /// <summary>
        /// 支付平台退款金额
        /// </summary>
        public decimal? RefundAmount{get;set;}

        /// <summary>
        /// 错误信息
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// 操作地址
        /// </summary>
        [MaxLength(100)]
        [Required]
        public string OpAddress { get; set; }

        /// <summary>
        /// 操作设备
        /// </summary>
        public ClientDeviceType OpDevice { get; set; }

        /// <summary>
        /// 根业务
        /// </summary>
        [MaxLength(100)]
        [Required]
        public string BizRoot { get; set; }


        /// <summary>
        /// 父业务
        /// </summary>
        [MaxLength(100)]
        [Index(IsUnique =true)]
        public string BizParent { get; set; }


        [InverseProperty(nameof(DataSettlementRollbackItemRecord.SettlementRollbackRecord))]
        public ICollection<DataSettlementRollbackItemRecord> Items { get; set; }


    }
}
