using System;
using System.ComponentModel.DataAnnotations;
using SF.Sys.Entities.Models;
using SF.Sys.Annotations;
using SF.Sys.Auth;
using SF.Sys.Clients;
using SF.Sys.Services.Management.Models;
using System.ComponentModel;

namespace SF.Biz.Accounting
{
    public enum DepositState
	{
        /// <summary>
        /// 处理中
        /// </summary>
        Processing,

        /// <summary>
        /// 已充值未退款
        /// </summary>
        Completed,

        /// <summary>
        /// 失败
        /// </summary>
        Failed,


        /// <summary>
        /// 退款中
        /// </summary>
        Refunding,

        /// <summary>
        /// 已退款
        /// </summary>
        Refunded,

        /// <summary>
        /// 已处理
        /// </summary>
        [Ignore]
        AfterProcessing,

        /// <summary>
        /// 已充值含退款
        /// </summary>

        CompletedWithRefund
    }

    /// <summary>
    /// 账户充值记录
    /// </summary>
    [EntityObject]
    public class DepositRecord : EventEntityBase
	{

        /// <summary>
        /// 用户
        /// </summary>
        [EntityIdent(typeof(User), nameof(DstName))]
        public long DstId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [TableVisible]
        [Ignore]
        public string DstName { get; set; }

        /// <summary>
        /// 科目
        /// </summary>
        [EntityIdent(typeof(AccountTitle),nameof(AccountTitle))]
        public long AccountTitleId { get; set; }

        /// <summary>
        /// 科目
        /// </summary>
        [Ignore]
        [TableVisible]
        public string AccountTitle { get; set; }


        /// <summary>
        /// 结束时间
        /// </summary>
        [TableVisible]
        public DateTime? CompletedTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [TableVisible]
        public decimal Amount { get; set; }

        /// <summary>
        /// 余额
        /// </summary>
        [TableVisible]
        public decimal? CurValue { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [TableVisible]
        public DepositState State { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [TableVisible]
        [Layout(10)]
        public string Title { get; set; }

        /// <summary>
        /// 操作地址
        /// </summary>
        [TableVisible]
        public string OpAddress { get; set; }

        /// <summary>
        /// 操作设备
        /// </summary>
        [TableVisible]
        public ClientDeviceType OpDevice { get; set; }

        /// <summary>
        /// 支付收款记录
        /// </summary>
        [EntityIdent(typeof(Payments.CollectRecord))]
        public long CollectRecordId { get; set; }

        /// <summary>
        /// 支付平台
        /// </summary>
        [EntityIdent(typeof(ServiceInstance),nameof(PaymentPlatformName))]
        public long PaymentPlatformId { get; set; }

        /// <summary>
        /// 支付平台
        /// </summary>
        [Ignore]
        [TableVisible]
        public string PaymentPlatformName { get; set; }

        /// <summary>
        /// 支付描述
        /// </summary>
        public string PaymentDesc { get; set; }


 
        /// <summary>
        /// 错误信息
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// 退款申请金额
        /// </summary>
        public decimal RefundRequest { get; set; }
        /// <summary>
        /// 退款成功金额
        /// </summary>
        public decimal RefundSuccess { get; set; }
        /// <summary>
        /// 最后退款申请时间
        /// </summary>
        public DateTime? LastRefundRequestTime { get; set; }
        /// <summary>
        /// 最后退款成功时间
        /// </summary>
        public DateTime? LastRefundSuccessTime { get; set; }
        /// <summary>
        /// 最后退款原因
        /// </summary>
        public string LastRefundReason { get; set; }

    }
}
