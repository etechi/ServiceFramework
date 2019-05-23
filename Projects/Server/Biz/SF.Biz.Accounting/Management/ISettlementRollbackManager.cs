using SF.Biz.Payments;
using SF.Sys.Annotations;
using SF.Sys.Auth;
using SF.Sys.Clients;
using SF.Sys.Entities;
using SF.Sys.NetworkService;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace SF.Biz.Accounting
{
    public class SettlementRollbackRecordQueryArguments :QueryArgument
    {
        /// <summary>
        /// 状态
        /// </summary>
        public SettlementRollbackState? State { get; set; }

        /// <summary>
        /// 买家
        /// </summary>
        [EntityIdent(typeof(User))]
        public long? BuyerId { get; set; }

        /// <summary>
        /// 卖家
        /// </summary>
        [EntityIdent(typeof(User))]
        public long? SellerId { get; set; }


        /// <summary>
        /// 支付平台
        /// </summary>
        [EntityIdent(typeof(PaymentPlatform))]
        public long? RefundPaymentPlatformId { get; set; }


        /// <summary>
        /// 时间
        /// </summary>
        public DateQueryRange CreatedTime { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public QueryRange<decimal> Amount { get; set; }
    }


    public class SettlementRollbackCreateArgument
    {
        public long SettlementId { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public decimal Amount { get; set; }
        public ClientInfo ClientInfo { get; set; }
        public TrackIdent BizRoot { get; set; }
        public TrackIdent BizParent { get; set; }
    }

    public enum SettlementRollbackState
    {
        Processing,
        Success,
        Failed
    }
    public class SettlementRollbackStatus
    {
        public long Id { get; set; }
        public SettlementRollbackState State { get; set; }
        public string Message { get; set; }
        public DateTime? Expires { get; set; }
    }

    /// <summary>
    /// 结算记录管理器
    /// </summary>
    [NetworkService]
    [EntityManager]
    [DefaultAuthorize(PredefinedRoles.财务专员)]
    [DefaultAuthorize(PredefinedRoles.系统管理员)]
    public interface ISettlementRollbackManager :
        IEntitySource<ObjectKey<long>, SettlementRollbackRecord, SettlementRollbackRecordDetail, SettlementRollbackRecordQueryArguments>
    {
        Task<SettlementRollbackStatus> Create(SettlementRollbackCreateArgument Argument);
        Task<SettlementRollbackStatus> UpdateAndQueryStatus(long Id,ClientInfo clientInfo=null);

    }
}
