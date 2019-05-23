using SF.Sys.Annotations;
using SF.Sys.Auth;
using SF.Sys.Entities;
using SF.Sys.NetworkService;
using System.Threading.Tasks;

namespace SF.Common.Addresses.Management
{
    public class UserAddressQueryArguments : ObjectQueryArgument
    {
        /// <summary>
        /// 用户
        /// </summary>
        [EntityIdent(typeof(User))]
        public long? OwnerId { get; set; }

        /// <summary>
        /// 地区
        /// </summary>
        [EntityIdent(typeof(LocationInternal))]
        public int? DistinctId { get; set; }

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
    public interface IUserAddressManager :
        IEntitySource<ObjectKey<long>, UserAddressInternal, UserAddressQueryArguments>,
        IEntityManager<ObjectKey<long>, UserAddressInternal>
    {

    }
}
