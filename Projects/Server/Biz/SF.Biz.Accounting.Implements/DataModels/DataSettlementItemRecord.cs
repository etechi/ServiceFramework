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
    /// 结算明细记录
    /// </summary>
    public class DataSettlementItemRecord : DataEntityBase
    {
        [Index("rec",IsUnique =true,Order =1)]
        public long SettlementRecordId { get; set; }

        [ForeignKey(nameof(SettlementRecordId))]
        public DataSettlementRecord SettlementRecord { get; set; }

        [Index("rec", IsUnique = true, Order = 2)]
        public int Index { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public SettlementState State { get; set; }

        
        [Index]
        public long TitleId { get; set; }

        /// <summary>
        /// 总金额
        /// </summary>
        public decimal TotalAmount { get; set; }
        /// <summary>
        /// 剩余金额
        /// </summary>
        public decimal LeftAmount { get; set; }
        /// <summary>
        /// 退回金额
        /// </summary>
        public decimal RollbackAmount { get; set; }

        
        public long? TransferRecordId { get; set; }

        [ForeignKey(nameof(TransferRecordId))]
        public TransferRecord TransferRecord { get; set; }
    }
}
