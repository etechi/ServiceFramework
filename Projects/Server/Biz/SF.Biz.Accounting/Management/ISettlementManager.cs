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
    public class SettlementRecordQueryArguments :QueryArgument
    {
        /// <summary>
        /// 状态
        /// </summary>
        public SettlementState? State { get; set; }

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
        /// 中间科目
        /// </summary>
        [EntityIdent(typeof(AccountTitle))]
        public long? RepayTitleId { get; set; }

        /// <summary>
        /// 目标科目
        /// </summary>
        [EntityIdent(typeof(AccountTitle))]
        public long? DstTitleId { get; set; }

        /// <summary>
        /// 支付平台
        /// </summary>
        [EntityIdent(typeof(PaymentPlatform))]
        public long? PaymentPlatformId { get; set; }


        /// <summary>
        /// 时间
        /// </summary>
        public DateQueryRange CreatedTime { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public QueryRange<decimal> Amount { get; set; }
    }

    public class SettlementItemArgument
    {
        public decimal Amount { get; set; }
        public string AccountTitle { get; set; }
    }

    public enum SettlementState
    {
        Processing,
        WaitConfirm,
        Success,
        Failed,
        Cancelled
    }
    public class SettlementStartArgument
    {
        public string Name { get; set; }
        public string Desc { get; set; }
        public long BuyerId { get; set; }
        public long SellerId { get; set; }
        public string PrepayTitle { get; set; }
        public string DstTitle { get; set; }

        public decimal TotalAmount { get; set; }

        public string CollectTitle { get; set; }
        public string CollectHttpRedirect { get; set; }
        public long? CollectPaymentPlatformId { get; set; }


        public ClientInfo ClientInfo { get; set; }
        public TrackIdent BizRoot { get; set; }
        public TrackIdent BizParent { get; set; }

        public SettlementItemArgument[] Items { get; set; }
    }
    
    public class SettlementStatus
    {
        public long Id { get; set; }
        public SettlementState State { get; set; }
        public DateTime? Expires { get; set; }
        public string Message { get; set; }
        public IReadOnlyDictionary<string, string> CollectStartResult { get; set; }
    }


    /// <summary>
    /// 结算记录管理器
    /// </summary>
    [NetworkService]
    [EntityManager]
    [DefaultAuthorize(PredefinedRoles.财务专员)]
    [DefaultAuthorize(PredefinedRoles.系统管理员)]
    public interface ISettlementManager :
        IEntitySource<ObjectKey<long>, SettlementRecord,SettlementRecordDetail, SettlementRecordQueryArguments>
    {
        Task<SettlementStatus> Start(SettlementStartArgument Argument);
        Task Cancel(long Id, ClientInfo ClientInfo = null);
        Task<SettlementStatus> UpdateAndQueryStatus(long Id,ClientInfo ClientInfo=null);
        Task<SettlementStatus> Confirm(long Id, ClientInfo ClientInfo=null);

    }
}
