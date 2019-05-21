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
    /// 结算记录
    /// </summary>
    public class DataSettlementRecord : DataObjectEntityBase
    {
        /// <summary>
        /// 状态
        /// </summary>
        public SettlementState State { get; set; }
        
        /// <summary>
        /// 描述
        /// </summary>
        [MaxLength(100)]
        public string Desc { get; set; }

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
        /// 总结算数额
        /// </summary>
        public decimal TotalAmount { get; set; }
        /// <summary>
        /// 剩余数额
        /// </summary>
        public decimal LeftAmount { get; set; }
        /// <summary>
        /// 退回数额
        /// </summary>
        public decimal RollbackAmount { get; set; }

        /// <summary>
        /// 预收科目
        /// </summary>
        [Index]
        public long PrepayTitleId { get; set; }

        /// <summary>
        /// 目标科目
        /// </summary>
        [Index]
        public long DstTitleId { get; set; }


        /// <summary>
        /// 确认转账记录ID
        /// </summary>
        public long? LastTransferRecordId { get; set; }


        [Index]
        public long? CollectPaymentPlatformId { get; set; }

        public long? CollectRecordId { get; set; }

        public long? CollectTitleId { get; set; }

        public decimal CollectAmount { get; set; }

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

        [InverseProperty(nameof(DataSettlementItemRecord.SettlementRecord))]
        public ICollection<DataSettlementItemRecord> Items { get; set; }
    }
}
