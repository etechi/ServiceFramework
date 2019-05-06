using System;
using SF.Sys.Annotations;
using SF.Sys.Services.Management.Models;
using SF.Sys.Clients;
using SF.Sys.Entities.Models;

namespace SF.Biz.Payments.Managers
{
    [EntityObject]
    public class RefundRecord : EventEntityBase<string>
    {
    
        /// <summary>
        /// 收款编号
        /// </summary>
        [TableVisible]
        public string CollectIdent { get; set; }

        /// <summary>
        /// 第三方收款编号
        /// </summary>
        [TableVisible]
        public string CollectExtIdent { get; set; }


        /// <summary>
        /// 状态
        /// </summary>
        [TableVisible]
        public PaymentRefundState State { get; set; }

        /// <summary>
        /// 最后更新
        /// </summary>
        [TableVisible]
        public DateTime? LastUpdateTime { get; set; }

        /// <summary>
        /// 支付服务
        /// </summary>
        [EntityIdent(typeof(ServiceInstance),nameof(PaymentPlatformName))]
        public int PaymentPlatformId { get; set; }

        /// <summary>
        /// 支付服务
        /// </summary>
        [TableVisible]
        [Ignore]
        public string PaymentPlatformName { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [TableVisible]
        public string Title { get; set; }

       

        /// <summary>
        /// 金额
        /// </summary>
        [TableVisible]
        public decimal Amount { get; set; }


        /// <summary>
        /// 错误信息
        /// </summary>
        [MultipleLines]
        public string Error { get; set; }
    }

    public class RefundRecordDetail : RefundRecord
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 提交时间
        /// </summary>
        public DateTime? SubmitTime { get; set; }

        /// <summary>
        /// 完成时间
        /// </summary>
        public DateTime? CompletedTime { get; set; }

        /// <summary>
        /// 客户端类型
        /// </summary>
        public string ClientType { get; set; }

        /// <summary>
        /// 业务来源
        /// </summary>
        [EntityIdent]
        public string TrackEntityIdent { get; set; }

        /// <summary>
        /// 第三方平台单号
        /// </summary>
        public string ExtIdent { get; set; }



        /// <summary>
        /// 用户地址
        /// </summary>
        [TableVisible]
        public string OpAddress { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        [TableVisible]
        public ClientDeviceType OpDevice { get; set; }
    }
}
