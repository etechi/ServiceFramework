using SF.Sys.Auth;
using SF.Sys.Entities;
using System.ComponentModel.DataAnnotations;
using SF.Sys.Annotations;
using SF.Sys.Services.Management.Models;

namespace SF.Biz.Accounting
{

    public class DepositRecordQueryArguments : QueryArgument
    {

        /// <summary>
        /// 用户
        /// </summary>
        [EntityIdent(typeof(User))]
        public long? OwnerId { get; set; }

        /// <summary>
        /// 充值时间
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
        public int? TitleId { get; set; }

        /// <summary>
        /// 充值状态
        /// </summary>
        public DepositState? State { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public QueryRange<decimal> Amount { get; set; }
    }

    public interface IDepositRecordManager :
       IEntitySource<ObjectKey<long>, DepositRecord, DepositRecordQueryArguments>
    {

    }
}
