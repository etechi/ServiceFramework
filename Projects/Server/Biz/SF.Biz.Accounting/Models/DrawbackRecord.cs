using System;
using System.ComponentModel.DataAnnotations;
using SF.Sys.Entities.Models;
using SF.Sys.Annotations;
using SF.Sys.Auth;
using SF.Sys.Clients;
using SF.Sys.Services.Management.Models;

namespace SF.Biz.Accounting
{
    public enum DrawbackState
    {
        /// <summary>
        /// 提交中
        /// </summary>
        Sending,

        /// <summary>
        /// 处理中
        /// </summary>
        Processing,

        /// <summary>
        /// 已完成
        /// </summary>
        Success,

        /// <summary>
        /// 错误
        /// </summary>
        Error
    }

    /// <summary>
    /// 账户退款记录
    /// </summary>
    [EntityObject]
    public class DrawbackRecord : EventEntityBase
	{
        
        [Ignore]
        public long DepositRecordId { get; set; }

        /// <summary>
        /// 充值时间
        /// </summary>
        [TableVisible]
        public DateTime DepositRecordCreateTime { get; set; }

        /// <summary>
        /// 用户
        /// </summary>
        [EntityIdent(typeof(User), nameof(SrcName))]
        public int SrcId { get; set; }

        /// <summary>
        /// 用户
        /// </summary>
        [TableVisible]
        [Ignore]
        public string SrcName { get; set; }

        /// <summary>
        /// 科目
        /// </summary>
        [EntityIdent(typeof(AccountTitle),nameof(AccountTitle))]
        [Display(Name="")]
        [Layout(2, 2)]
        public int AccountTitleId { get; set; }

        /// <summary>
        /// 科目
        /// </summary>
        [Ignore]
        public string AccountTitle { get; set; }

        /// <summary>
        /// 退款开始
        /// </summary>
        [TableVisible]
        public DateTime CreatedTime { get; set; }

        /// <summary>
        /// 退款结束
        /// </summary>
        [TableVisible]
        public DateTime? CompletedTime { get; set; }

        /// <summary>
        /// 金额
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
        [Display(Name = "")]
        [Layout(4, 2)]
        public DrawbackState State { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [TableVisible]
        public string Title { get; set; }

        /// <summary>
        /// 操作地址
        /// </summary>
        public string OpAddress { get; set; }

        /// <summary>
        /// 操作设备
        /// </summary>
        public ClientDeviceType OpDevice { get; set; }


        /// <summary>
        /// 支付平台
        /// </summary>
        [EntityIdent(typeof(ServiceInstance),nameof(PaymentPlatformName))]
        public long PaymentPlatformId { get; set; }

        /// <summary>
        /// 支付平台
        /// </summary>
        [TableVisible]
        [Ignore]
        public string PaymentPlatformName { get; set; }

        /// <summary>
        /// 支付退款记录
        /// </summary>
        [EntityIdent(typeof(Payments.RefundRecord))]
        public long PaymentsRefundRecordId { get; set; }


        /// <summary>
        /// 支付描述
        /// </summary>
        public string PaymentDesc { get; set; }

        /// <summary>
        /// 业务来源
        /// </summary>
        [EntityIdent]
        public string TrackEntityIdent { get; set; }

        /// <summary>
        /// 退款原因
        /// </summary>
        [TableVisible]
        public string Reason { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        [TableVisible]
        public string Error { get; set; }

    }
}
