using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SF.Sys.Entities.DataModels;
using SF.Sys.Data;
using SF.Sys.Clients;

namespace SF.Biz.Accounting.DataModels
{

    /// <summary>
    /// 充值记录
    /// </summary>
    public class DataDepositRecord : DataEventEntityBase
	{

        /// <summary>
        /// 账户科目ID
        /// </summary>
        [Index("owner",Order = 1)]
		[Index("op", Order = 1)]
        [Display(Name = "")]
        public long AccountTitleId { get; set; }

        /// <summary>
        /// 目标账户
        /// </summary>
		[Index("owner", Order = 2)]
        public long DstId { get; set; }



		[ForeignKey(nameof(AccountTitleId))]
		public DataAccountTitle AccountTitle { get; set; }

        /// <summary>
        /// 操作者ID
        /// </summary>
        [Index("op",Order =2)]
        public long OperatorId { get; set; }

        /// <summary>
        /// 充值金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 实际收款金额
        /// </summary>
        public decimal AmountCollected { get; set; }
        /// <summary>
        /// 充值后账户余额
        /// </summary>
        public decimal? CurValue { get; set; }

        /// <summary>
        /// 充值标题
        /// </summary>
		[MaxLength(200)]
		[Required]
        public string Title { get; set; }

        /// <summary>
        /// 业务跟踪ID
        /// </summary>
		[MaxLength(100)]
		[Required]
		[Index(IsUnique =true)]
        public string TrackEntityIdent { get; set; }


        /// <summary>
        /// 提醒Id
        /// </summary>
        public long RemindId { get; set; }


        /// <summary>
        /// 完成时间
        /// </summary>
        public DateTime? CompletedTime { get; set; }

        /// <summary>
        /// 充值状态
        /// </summary>
        public DepositState State { get; set; }

        /// <summary>
        /// 支付平台ID
        /// </summary>
        public long PaymentPlatformId { get; set; }

        /// <summary>
        /// 支付描述
        /// </summary>
		[MaxLength(100)]
        public string PaymentDesc { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
		[MaxLength(200)]
        public string Error { get; set; }

        /// <summary>
        /// 操作地址
        /// </summary>
        [MaxLength]
        public string OpAddress { get; set; }

        /// <summary>
        /// 操作设备
        /// </summary>
        public ClientDeviceType OpDevice { get; set; }

        /// <summary>
        /// 退款请求总金额
        /// </summary>
        public decimal RefundRequest { get; set; }
        /// <summary>
        /// 退款成功总金额
        /// </summary>
        public decimal RefundSuccess { get; set; }
        /// <summary>
        /// 最后退款请求时间
        /// </summary>
        public DateTime? LastRefundRequestTime { get; set; }
        /// <summary>
        /// 最后退款成功时间
        /// </summary>
        public DateTime? LastRefundSuccessTime { get; set; }
        /// <summary>
        /// 最后退款原因
        /// </summary>
        [MaxLength(100)]
        public string LastRefundReason { get; set; }

        /// <summary>
        /// 支付模块收款ID
        /// </summary>
        public long CollectRecordId { get; set; }

        /// <summary>
        /// 乐观锁时间戳
        /// </summary>
        [ConcurrencyCheck]
        [Timestamp]
        public byte[] TimeStamp { get; set; }


    }
}
