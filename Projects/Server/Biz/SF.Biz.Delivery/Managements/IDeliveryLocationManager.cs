using SF.Sys.Annotations;
using SF.Sys.Auth;
using SF.Sys.Entities;
using SF.Sys.NetworkService;
using System.Threading.Tasks;

namespace SF.Biz.Delivery.Management
{
    public class DeliveryLocationQueryArguments : ObjectQueryArgument<int>
    {
        /// <summary>
        /// 父地区
        /// </summary>
        [EntityIdent(typeof(DeliveryLocation))]
        public int? ParentId { get; set; }

        /// <summary>
        /// 英文名
        /// </summary>
        public string EnName { get; set; }
        /// <summary>
        /// 全名
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// 代码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 电话区号
        /// </summary>
        public string PhonePrefix { get; set; }

        /// <summary>
        /// 级别
        /// </summary>
        public int? Level { get; set; }

    }

    /// <summary>
    /// 地区管理
    /// </summary>
    [NetworkService]
    [EntityManager]
    [DefaultAuthorize(PredefinedRoles.客服专员, true)]
    [DefaultAuthorize(PredefinedRoles.运营专员)]
    [DefaultAuthorize(PredefinedRoles.系统管理员)]
    public interface IDeliveryLocationManager :
        IEntitySource<ObjectKey<int>, DeliveryLocation, DeliveryLocationQueryArguments>,
        IEntityManager<ObjectKey<int>, DeliveryLocation>
    {
    }
}
