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
    /// 结算回退项目记录
    /// </summary>
    public class DataSettlementRollbackItemRecord : DataEntityBase
    {

        /// <summary>
        /// 结算ID
        /// </summary>
        [Index]
        public long SettlementRollbackRecordId { get; set; }

        [ForeignKey(nameof(SettlementRollbackRecordId))]
        public DataSettlementRollbackRecord SettlementRollbackRecord { get; set; }

        /// <summary>
        /// 回退数额
        /// </summary>
        public decimal Amount { get; set; }

        [Index]
        public long SettlementItemRecordId { get; set; }

        [ForeignKey(nameof(SettlementItemRecordId))]
        public DataSettlementItemRecord SettlementItemRecord { get;set;}

        /// <summary>
        /// 退回科目Id
        /// </summary>
        public long TitleId { get; set; }

        /// <summary>
        /// 转账记录
        /// </summary>
        public long TransferRecordId { get; set; }

    }
}
