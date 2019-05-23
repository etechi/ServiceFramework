using SF.Sys.Annotations;
using SF.Sys.Auth;
using SF.Sys.Entities;
using SF.Sys.NetworkService;
using System.Threading.Tasks;

namespace SF.Common.Addresses.Management
{
    public class AddressSnapshotQueryArguments : ObjectQueryArgument
    {
        /// <summary>
        /// 用户
        /// </summary>
        [EntityIdent(typeof(User))]
        public long? UserId { get; set; }

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
    /// 地址快照管理
    /// </summary>
    [NetworkService]
    [EntityManager]
    [DefaultAuthorize(PredefinedRoles.客服专员, true)]
    [DefaultAuthorize(PredefinedRoles.运营专员)]
    [DefaultAuthorize(PredefinedRoles.系统管理员)]
    public interface IAddressSnapshotManager :
        IEntitySource<ObjectKey<long>, AddressSnapshot, AddressSnapshot, AddressSnapshotQueryArguments>
    {
        Task<long> CreateFromAddress(long AddressId);

    }
}
