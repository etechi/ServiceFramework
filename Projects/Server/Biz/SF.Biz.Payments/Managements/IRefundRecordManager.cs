using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using SF.Sys.Entities;
using SF.Sys.Clients;
using SF.Sys.Annotations;
using SF.Sys.Auth;
using SF.Sys.Services.Management.Models;
using SF.Sys.NetworkService;

namespace SF.Biz.Payments.Managers
{

    public class RefundRecordQueryArgument : QueryArgument<string>
    {
        [Display(Name = "外部ID")]
        public string ExtIdent { get; set; }

        [Display(Name = "收款Id")]
        public string CollectIdent { get; set; }

        [Display(Name = "状态")]
        public RefundState? State { get; set; }

        [Display(Name = "时间")]
        public DateQueryRange Time { get; set; }

        [Display(Name = "支付平台")]
        [EntityIdent(typeof(ServiceInstance))]
        public int? PaymentPlatformId { get; set; }

        [EntityIdent(typeof(User))]
        [Display(Name = "用户")]
        public string UserId { get; set; }

    }

    /// <summary>
    /// 退款记录管理器
    /// </summary>
    [NetworkService]
    [EntityManager]
    [DefaultAuthorize(PredefinedRoles.财务专员)]
    [DefaultAuthorize(PredefinedRoles.系统管理员)]

    public interface IRefundRecordManager:
        IEntitySource<ObjectKey<long>, RefundRecord, RefundRecordDetail, RefundRecordQueryArgument>
    {
    }

}
