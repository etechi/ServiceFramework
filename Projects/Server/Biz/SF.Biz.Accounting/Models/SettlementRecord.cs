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
    /// 结算项目
    /// </summary>
    public class SettlementItemRecord :IEntityWithId<long>
    {
        /// <summary>
        /// ID
        /// </summary>
        [Key]
        public long Id { get; set; }
        
        /// <summary>
        /// 序号
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 科目
        /// </summary>
        [EntityIdent(typeof(AccountTitle))]
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
        
        /// <summary>
        /// 转账记录
        /// </summary>
        [EntityIdent(typeof(TransferRecord))]
        public long? TransferRecordId { get; set; }
    }

    /// <summary>
    /// 结算记录
    /// </summary>
    [EntityObject]
    public class SettlementRecord : ObjectEntityBase
    {

        /// <summary>
        /// 状态
        /// </summary>
        public SettlementState State { get; set; }

        /// <summary>
        /// 买家
        /// </summary>
        [EntityIdent(typeof(User), nameof(BuyerName))]
        [Uneditable]
        public long BuyerId { get; set; }

        ///<title>买家</title>
        /// <summary>
        /// 源用户为空为系统奖励操作
        /// </summary>
        [Ignore]
        [TableVisible]
        public string BuyerName { get; set; }



        ///<summary>卖家</summary>
        [EntityIdent(typeof(User), nameof(SellerName))]
        [Layout(3, 1)]
        [Uneditable]
        public long SellerId { get; set; }

        /// <summary>
        /// 卖家
        /// </summary>
        [Ignore]
        [TableVisible]
        public string SellerName { get; set; }

        /// <summary>
        /// 结算数额
        /// </summary>
        [TableVisible]
        [Uneditable]
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// 剩余数额
        /// </summary>
        [TableVisible]
        [Uneditable]
        public decimal LeftAmount { get; set; }

        /// <summary>
        /// 已退数额
        /// </summary>
        [TableVisible]
        [Uneditable]
        public decimal RollbackAmount { get; set; }

        /// <summary>
        /// 预收科目
        /// </summary>
        [EntityIdent(typeof(AccountTitle), nameof(PrepayTitleName))]
        [Uneditable]
        public long PrepayTitleId { get; set; }

        /// <summary>
        /// 中间科目
        /// </summary>
        [Ignore]
        [TableVisible]
        public string PrepayTitleName { get; set; }


        /// <summary>
        /// 目标科目
        /// </summary>
        [EntityIdent(typeof(AccountTitle), nameof(DstTitleName))]
        [Uneditable]
        public long DstTitleId { get; set; }

        /// <summary>
        /// 目标科目
        /// </summary>
        [Ignore]
        [TableVisible]
        public string DstTitleName { get; set; }


        /// <summary>
        /// 支付平台
        /// </summary>
        [EntityIdent(typeof(PaymentPlatform), nameof(PaymentPlatformName))]
        [Uneditable]
        public long? PaymentPlatformId { get; set; }

        /// <summary>
        /// 支付平台
        /// </summary>
        [Ignore]
        [TableVisible]
        public string PaymentPlatformName { get; set; }

        /// <summary>
        /// 收款记录
        /// </summary>
        [EntityIdent(typeof(CollectRecord))]
        [ReadOnly(true)]
        public long? CollectRecordId { get; set; }

        /// <summary>
        /// 收款金额
        /// </summary>
        public decimal CollectAmount { get; set; }
    }
    public class SettlementRecordDetail : SettlementRecord
    { 
        /// <summary>
        /// 操作地址
        /// </summary>
        [Uneditable]
        public string OpAddress { get; set; }

        /// <summary>
        /// 操作设备
        /// </summary>
        [Uneditable]
        public ClientDeviceType OpDevice { get; set; }

        /// <summary>
        /// 根业务
        /// </summary>
        [EntityIdent(NameField: nameof(BizRootName), WithBizType = true)]
        [Uneditable]
        public string BizRoot { get; set; }

        /// <summary>
        /// 根业务
        /// </summary>
        [Ignore]
        [TableVisible]
        public string BizRootName { get; set; }

        /// <summary>
        /// 父业务
        /// </summary>
        [EntityIdent(NameField: nameof(BizParentName), WithBizType = true)]
        [Uneditable]
        public string BizParent { get; set; }

        /// <summary>
        /// 父业务
        /// </summary>
        [Ignore]
        [TableVisible]
        public string BizParentName { get; set; }

        
        [TableVisible]
        [JsonData]
        public SettlementItemRecord[] Source { get; set; }
    }

}
