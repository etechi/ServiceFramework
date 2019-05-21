using SF.Sys.Annotations;
using SF.Sys.Auth;
using SF.Sys.Clients;
using SF.Sys.Entities;
using SF.Sys.Entities.Models;
using SF.Sys.Reminders.Models;
using SF.Sys.Services.Management.Models;
using System;
using System.ComponentModel.DataAnnotations;
namespace SF.Biz.Payments
{
    /// <summary>
    /// 收款记录
    /// </summary>
    [EntityObject]
    public class CollectRecord : EventEntityBase
    {

        /// <summary>
        /// 状态
        /// </summary>
        [TableVisible]
        public CollectState State { get; set; }

        /// <summary>
        /// 支付服务
        /// </summary>
        [EntityIdent(typeof(ServiceInstance),nameof(PaymentPlatformName))]
        public long PaymentPlatformId { get; set; }


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

    public class CollectRecordDetail : CollectRecord
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }
        /// <summary>
        /// 完成时间
        /// </summary>
        public DateTime? CompletedTime { get; set; }


        /// <summary>
        /// 客户端类型
        /// </summary>
        public string ClientType { get; set; }

        
        /// <summary>
        /// 根业务
        /// </summary>
        [EntityIdent( WithBizType =true)]
        public string BizRoot { get; set; }

        /// <summary>
        /// 父业务
        /// </summary>
        [EntityIdent(WithBizType = true)]
        public string BizParent { get; set; }

        /// <summary>
        /// 第三方平台单号
        /// </summary>
        public string ExtIdent { get; set; }

        /// <summary>
        /// 第三方平台用户ID
        /// </summary>
        public string ExtUserId { get; set; }

        /// <summary>
        /// 第三方平台用户名
        /// </summary>
        public string ExtUserName { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        [MultipleLines]
        public string Error { get; set; }


        /// <summary>
        /// 提醒
        /// </summary>
        [EntityIdent(typeof(Reminder))]
        public long RemindId { get; set; }

    }
}
