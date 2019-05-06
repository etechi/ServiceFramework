using SF.Sys.Annotations;
using SF.Sys.Auth;
using SF.Sys.Clients;
using SF.Sys.Entities;
using SF.Sys.Services.Management.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace SF.Biz.Accounting
{
  
    public class RefundRecordQueryArguments:QueryArgument
    {
        /// <summary>
        /// 充值记录
        /// </summary>
        [EntityIdent(typeof(DepositRecord))]
        public long? DepositIdent { get; set; }


        /// <summary>
        /// 用户
        /// </summary>
        [EntityIdent(typeof(User))]
        public int? OwnerId { get; set; }

        /// <summary>
        /// 退款时间
        /// </summary>
        public DateQueryRange Time { get; set; }

        /// <summary>
        /// 充值平台
        /// </summary>
        [EntityIdent(typeof(ServiceInstance))]
        public long? PaymentPlatformId { get; set; }

        /// <summary>
        /// 科目
        /// </summary>
        [EntityIdent(typeof(AccountTitle))]
        public long? TitleId { get; set; }

        /// <summary>
        /// 退款状态
        /// </summary>
        public RefundState? State { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public QueryRange<decimal> Amount { get; set; }
    }

    public interface IRefundRecordManager :
        IEntitySource<ObjectKey<long>,RefundRecord,RefundRecordQueryArguments>
    {

    }
}
