using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SF.Sys.Entities.DataModels;
using SF.Sys.Data;
using SF.Sys.Clients;

namespace SF.Biz.Accounting.DataModels
{
    /// <summary>
    /// 退款记录
    /// </summary>
    public class DataRefundRecord : DataEventEntityBase
    {

        /// <summary>
        /// 相关充值记录ID
        /// </summary>
        [Index]
        public long DepositRecordId { get; set; }

        [ForeignKey(nameof(DepositRecordId))]
        public DataDepositRecord DepositRecord { get; set; }

        /// <summary>
        /// 相关充值记录创建时间
        /// </summary>
        public DateTime DepositRecordCreateTime { get; set; }

        /// <summary>
        /// 账户科目ID
        /// </summary>
        [Index("owner", Order = 1)]
        [Index("op", Order = 1)]
        public long AccountTitleId { get; set; }

        /// <summary>
        /// 退款用户ID
        /// </summary>
		[Index("owner", Order = 2)]
        public long SrcId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
		[Index("owner", Order = 3)]
        [Index("op", Order = 3)]
        public DateTime CreatedTime { get; set; }

        [ForeignKey(nameof(AccountTitleId))]
        public DataAccountTitle AccountTitle { get; set; }

        /// <summary>
        /// 操作者ID
        /// </summary>
        [Index("op", Order = 2)]
        public long OperatorId { get; set; }

        /// <summary>
        /// 退款金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 退款成功后账户余额
        /// </summary>
        public decimal? CurValue { get; set; }

        /// <summary>
        /// 退款标题
        /// </summary>
		[MaxLength(200)]
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// 业务跟踪标识
        /// </summary>
		[MaxLength(100)]
        [Required]
        [Index("TraceEntityId", Order = 1, IsUnique = true)]
        public string TrackEntityIdent { get; set; }

        ///<title>业务跟踪标识索引</title>
        /// <summary>
        /// 当一个业务有多次退款时，使用此字段区分
        /// </summary>
        [Index("TraceEntityId", Order = 2, IsUnique = true)]
        public int TrackEntityIndex { get; set; }

        /// <summary>
        /// 回调过程名
        /// </summary>
        [MaxLength(50)]
        public string CallbackName { get; set; }

        /// <summary>
        /// 回调上下文
        /// </summary>
		[MaxLength(100)]
        public string CallbackContext { get; set; }

        /// <summary>
        /// 第三方平台提交时间
        /// </summary>
        public DateTime? SubmittedTime { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdatedTime { get; set; }
        /// <summary>
        /// 完成时间
        /// </summary>
        public DateTime? CompletedTime { get; set; }

        /// <summary>
        /// 退款状态
        /// </summary>
        public RefundState State { get; set; }

        /// <summary>
        /// 退款平台ID
        /// </summary>
        public long PaymentPlatformId { get; set; }

        /// <summary>
        /// 退款描述
        /// </summary>
		[MaxLength(100)]
        public string PaymentDesc { get; set; }

        /// <summary>
        /// 退款原因
        /// </summary>
        [MaxLength(100)]
        public string Reason { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
		[MaxLength(200)]
        public string Error { get; set; }

        /// <summary>
        /// 操作地址
        /// </summary>
        [MaxLength(200)]
        public string OpAddress { get; set; }

        /// <summary>
        /// 操作设备
        /// </summary>
        public ClientDeviceType OpDevice { get; set; }

        /// <summary>
        /// 支付退款记录
        /// </summary>
        public long PaymentsRefundRecordId{get;set;}

        /// <summary>
        /// 乐观锁时间戳
        /// </summary>
        [ConcurrencyCheck]
        [Timestamp]
        public byte[] TimeStamp { get; set; }

    }
}
