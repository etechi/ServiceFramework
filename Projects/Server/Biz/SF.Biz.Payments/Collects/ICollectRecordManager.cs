using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using SF.Sys.Clients;
using SF.Sys.Entities;
using SF.Sys.Annotations;
using SF.Sys.Auth;
using SF.Sys.Services.Management.Models;

namespace SF.Biz.Payments.Managers
{

    public class CollectRecordQueryArgument : QueryArgument
    {

        [Display(Name = "外部ID")]
        public string ExtIdent { get; set; }

        [Display(Name = "状态")]
        public CollectState? State { get; set; }

        [Display(Name = "时间")]
        public DateQueryRange Time { get; set; }

        [Display(Name = "支付平台")]
        [EntityIdent(typeof(ServiceInstance))]
        public int? PaymentPlatformId { get; set; }

        [EntityIdent(typeof(User))]
        [Display(Name = "用户")]
        public long UserId { get; set; }

    }
    public interface ICollectRecordManager:
        IEntitySource<ObjectKey<long>, CollectRecord, CollectRecordDetail, CollectRecordQueryArgument>
    {
    }

}
