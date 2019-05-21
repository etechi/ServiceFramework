using SF.Biz.Payments;
using SF.Sys.Annotations;
using SF.Sys.Auth;
using SF.Sys.Clients;
using SF.Sys.Entities;
using SF.Sys.Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SF.Biz.Accounting
{

    /// <summary>
    /// 结算退款项目
    /// </summary>
    public class SettlementRollbackItemRecord : IEntityWithId<long>
    {
        /// <summary>
        /// Id
        /// </summary>
        [Key]
        public long Id { get; set; }

        /// <summary>
        /// 科目
        /// </summary>
        [EntityIdent(typeof(AccountTitle))]
        public long TitleId { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 转账记录
        /// </summary>
        [EntityIdent(typeof(TransferRecord))]
        public long TransferRecordId { get; set; }

    }

    /// <summary>
    /// 结算退款记录
    /// </summary>
    [EntityObject]
    public class SettlementRollbackRecord : ObjectEntityBase
    {


        /// <summary>
        /// 状态
        /// </summary>
        [TableVisible]
        public SettlementRollbackState State { get; set; }

        /// <summary>
        /// 买家
        /// </summary>
        [EntityIdent(typeof(User), nameof(BuyerName))]
        public long BuyerId { get; set; }

        /// <summary>
        /// 买家
        /// </summary>
        [Ignore]
        [TableVisible]
        public string BuyerName { get; set; }

        /// <summary>
        /// 卖家
        /// </summary>
        [EntityIdent(typeof(User), nameof(SellerName))]
        public long SellerId { get; set; }
        /// <summary>
        /// 卖家
        /// </summary>
        [Ignore]
        [TableVisible]
        public string SellerName { get; set; }

        /// <summary>
        /// 结算记录
        /// </summary>
        [EntityIdent(typeof(SettlementRecord), nameof(SettlementRecordName))]
        public long SettlementRecordId { get; set; }

        [TableVisible]
        [Ignore]
        public string SettlementRecordName { get; set; }

        /// <summary>
        /// 总回退数额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 退款记录
        /// </summary>
        [EntityIdent(typeof(RefundRecord))]
        public long? RefundRecordId { get; set; }

        /// <summary>
        /// 退款支付平台
        /// </summary>
        [EntityIdent(typeof(PaymentPlatform))]
        public long? RefundPlatformPlatformId { get; set; }

        /// <summary>
        /// 退款科目
        /// </summary>
        [EntityIdent(typeof(AccountTitle))]
        public long? RefundTitleId { get; set; }

        /// <summary>
        /// 支付平台退款
        /// </summary>
        public decimal? RefundAmount { get; set; }

    }
    public class SettlementRollbackRecordDetail : SettlementRollbackRecord
    { 

        /// <summary>
        /// 描述
        /// </summary>
        [MaxLength(100)]
        public string Desc { get; set; }


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
        public string BizParent { get; set; }


        [TableRows]
        public IEnumerable<SettlementRollbackItemRecord> Items { get; set; }

    }

}
