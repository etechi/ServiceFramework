using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SF.Sys.Entities.DataModels;
using SF.Sys.Data;
using SF.Sys.Clients;

namespace SF.Biz.Payments.DataModels
{

    /// <summary>
    /// 第三方退款记录
    /// </summary>
    public class DataRefundRecord:DataEventEntityBase
    {
	
        /// <summary>
        /// 退款系统服务ID
        /// </summary>
        [Index]
        public long PaymentPlatformId { get; set; }


        /// <summary>
        /// 对应收款交易号
        /// </summary>
        [Index]
        public long CollectIdent { get; set; }

        /// <summary>
        /// 对应第三方收款交易号
        /// </summary>
        [MaxLength(100)]
        public string CollectExtIdent { get; set; }

        /// <summary>
        /// 退款标题
        /// </summary>
		[Required]
		[MaxLength(100)]
        public string Title { get; set; }

        /// <summary>
        /// 退款描述
        /// </summary>
		[MaxLength(100)]
        public string Desc { get; set; }

        /// <summary>
        /// 回调名称
        /// </summary>
		[MaxLength(50)]
        public string CallbackName { get; set; }

        /// <summary>
        /// 回调上下文
        /// </summary>
		[MaxLength(100)]
        public string CallbackContext { get; set; }

        /// <summary>
        /// 第三方平台退款交易号
        /// </summary>
		[MaxLength(100)]
        [Index]
        public string ExtIdent { get; set; }

        /// <summary>
        /// 退款状态
        /// </summary>
        [Index]
        public RefundState State { get; set; }


        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }
        /// <summary>
        /// 提交时间
        /// </summary>
        public DateTime? SubmitTime { get; set; }
        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime? LastUpdateTime { get; set; }
        /// <summary>
        /// 完成时间
        /// </summary>

        public DateTime? CompletedTime { get; set; }

        /// <summary>
        /// 更新次数
        /// </summary>
        public int UpdateCount { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// 退款金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 实际退款金额
        /// </summary>
        public decimal AmountRefund { get; set; }

        /// <summary>
        /// 根业务
        /// </summary>
        [Index]
        [MaxLength(100)]
        [Required]
        public string BizRoot { get; set; }

        /// <summary>
        /// 父业务
        /// </summary>
        [Index(IsUnique = true)]
        [MaxLength(100)]
        [Required]
        public string BizParent { get; set; }


        /// <summary>
        /// 乐观锁时间戳
        /// </summary>
        [ConcurrencyCheck]
		[Timestamp]
        public byte[] TimeStamp { get; set; }

        /// <summary>
        /// 附加数据
        /// </summary>
        public string ExtraData { get; set; }

        /// <summary>
        /// 退款发起地址
        /// </summary>
        [MaxLength(100)]
        [Display(Name = "")]
        public string OpAddress { get; set; }

        /// <summary>
        /// 退款发起设备
        /// </summary>
        public ClientDeviceType OpDevice { get; set; }

    }
}
