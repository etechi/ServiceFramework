using SF.Common.Addresses;
using SF.Sys.Annotations;
using SF.Sys.Entities.Models;

namespace SF.Biz.Shops.Managements
{
    /// <summary>
    /// 店铺
    /// </summary>
    [EntityObject]
    public class ShopInternal : UIObjectEntityBase
    {
        /// <summary>
        /// 类型
        /// </summary>
        [EntityIdent(typeof(ShopTypeInternal),nameof(ShopTypeName))]
        public long ShopTypeId { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        [TableVisible]
        [Ignore]
        public string ShopTypeName { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        [EntityIdent(typeof(UserAddress),nameof(AddressName))]
        public long? AddressId { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        [Ignore]
        [TableVisible]
        public string AddressName { get; set; }

    }
}
