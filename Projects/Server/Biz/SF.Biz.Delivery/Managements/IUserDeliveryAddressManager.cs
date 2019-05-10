using SF.Sys.Annotations;
using SF.Sys.Auth;
using SF.Sys.Entities;
using SF.Sys.NetworkService;
using System.Threading.Tasks;

namespace SF.Biz.Delivery.Management
{
    public class UserDeliveryAddressQueryArguments : ObjectQueryArgument
    {
        /// <summary>
        /// 用户
        /// </summary>
        [EntityIdent(typeof(User))]
        public long? UserId { get; set; }

        /// <summary>
        /// 地区
        /// </summary>
        [EntityIdent(typeof(DeliveryLocation))]
        public long? DistinctId { get; set; }

        /// <summary>
        /// 默认地址
        /// </summary>
        public bool? IsDefaultAddress { get; set; }

    }

    /// <summary>
    /// 发货地址管理
    /// </summary>
    [NetworkService]
    [EntityManager]
    [DefaultAuthorize(PredefinedRoles.客服专员, true)]
    [DefaultAuthorize(PredefinedRoles.运营专员)]
    [DefaultAuthorize(PredefinedRoles.系统管理员)]
    public interface IUserDeliveryAddressManager :
        IEntitySource<ObjectKey<long>, UserDeliveryAddress, UserDeliveryAddressQueryArguments>,
        IEntityManager<ObjectKey<long>, UserDeliveryAddress>
    {
        Task<long> GetSnapshot(long Id);
        Task<DeliveryAddressDetail> QueryShapshotAddress(long Id);

    }
}
