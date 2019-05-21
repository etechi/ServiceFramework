using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SF.Sys.Data;
using SF.Sys.Clients;

namespace SF.Biz.Payments.DataModels
{


    /// <summary>
    /// 第三方收款记录
    /// </summary>
	public class DataCollectRecord : SF.Sys.Entities.DataModels.DataEventEntityBase
	{
        /// <summary>
        /// 收款系统服务ID
        /// </summary>
        [Index]
        public long PaymentPlatformId { get; set; }


        /// <summary>
        /// 收款结束重定向Url
        /// </summary>
		[MaxLength(200)]
        public string HttpRedirect { get; set; }

        /// <summary>
        /// 收款标题
        /// </summary>
		[Required]
		[MaxLength(100)]
        public string Title { get; set; }

        /// <summary>
        /// 收款描述
        /// </summary>
		[MaxLength(100)]
        public string Desc { get; set; }

        /// <summary>
        /// 提醒
        /// </summary>
        public long RemindId { get; set; }

        /// <summary>
        /// 第三方平台交易号
        /// </summary>
		[MaxLength(100)]
        [Index]
        public string ExtIdent { get; set; }

        /// <summary>
        /// 收款状态
        /// </summary>
        [Index]
        public CollectState State { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Index]
        public DateTime CreatedTime { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? CompletedTime { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// 收款金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 确认收款金额
        /// </summary>
        public decimal AmountCollected { get; set; }


        /// <summary>
        /// 第三方平台用户ID
        /// </summary>
        public string ExtUserId { get; set; }
        /// <summary>
        /// 第三方平台用户名
        /// </summary>
        public string ExtUserName { get; set; }
        /// <summary>
        /// 第三方平台用户账号
        /// </summary>
        public string ExtUserAccount { get; set; }

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
        /// 收款发起地址
        /// </summary>
        [MaxLength(100)]
        public string OpAddress { get; set; }

        /// <summary>
        /// 收款发起设备
        /// </summary>
        public ClientDeviceType OpDevice { get; set; }

    }
}
