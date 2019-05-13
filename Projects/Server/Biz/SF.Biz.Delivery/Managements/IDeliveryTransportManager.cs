using SF.Sys.Annotations;
using SF.Sys.Auth;
using SF.Sys.Entities;
using SF.Sys.Entities.Models;
using SF.Sys.NetworkService;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace SF.Biz.Delivery.Management
{

    /// <summary>
    /// 发货渠道
    /// </summary>
    [EntityObject]
    public class DeliveryTransport : UIObjectEntityBase
    {

        /// <summary>
        /// 联系人
        /// </summary>
        [TableVisible]
        [StringLength(8)]
        public string ContactName { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        [TableVisible]
        [StringLength(20)]
        public string ContactPhone { get; set; }

        /// <summary>
        /// 代号
        /// </summary>
        [TableVisible]
        [StringLength(20)]
        public string Code { get; set; }

        /// <summary>
        /// 网站
        /// </summary>
        [StringLength(100)]
        public string Site { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Order { get; set; }

    }
    public class DeliveryTransportQueryArguments : ObjectQueryArgument
    {
        /// <summary>
        /// 代码
        /// </summary>
        public string Code { get; set; }

    }

    /// <summary>
    /// 地区管理
    /// </summary>
    [NetworkService]
    [EntityManager]
    [DefaultAuthorize(PredefinedRoles.客服专员, true)]
    [DefaultAuthorize(PredefinedRoles.运营专员)]
    [DefaultAuthorize(PredefinedRoles.系统管理员)]
    public interface IDeliveryTransportManager :
        IEntitySource<ObjectKey<long>, DeliveryTransport, DeliveryTransportQueryArguments>,
        IEntityManager<ObjectKey<long>, DeliveryTransport>
    {
    }
}
