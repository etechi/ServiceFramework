using SF.Sys.Annotations;
using SF.Sys.NetworkService;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace SF.Biz.Delivery
{
    public class UserAddress
    {
        /// <summary>
        /// ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 地区
        /// </summary>
        public long LocationId { get; set; }

        /// <summary>
        /// 地区Id
        /// </summary>
        public string LocationName { get; set; }

        /// <summary>
        /// 邮编
        /// </summary>
        public string ZipCode { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        [StringLength(100)]
        public string Address { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        public string ContactName { get; set; }

        /// <summary>
        /// 联系人手机
        /// </summary>
        public string ContactPhoneNumber { get; set; }
       
    }
    public class UserAddressEditable
    {
        /// <summary>
        /// ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        public string ContactName { get; set; }

        /// <summary>
        /// 联系人手机
        /// </summary>
        public string ContactPhoneNumber { get; set; }

        [Ignore]
        public bool PhoneNumberVerified { get; set; }

        /// <summary>
        /// 省
        /// </summary>
        public long ProvinceId { get; set; }

        /// <summary>
        /// 市
        /// </summary>
        public long CityId { get; set; }

        /// <summary>
        /// 县/区
        /// </summary>
        public long DistrictId { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        [StringLength(100)]
        public string Address { get; set; }

        /// <summary>
        /// 默认地址
        /// </summary>
        public bool IsDefaultAddress { get; set; }
    }
    [NetworkService]
    public interface IUserDeliveryAddressService
    {
        Task<UserAddress> GetUserAddress(long? AddressId = null);
        Task<UserAddress[]> ListUserAddresses();
        Task<UserAddressEditable> LoadForEditAsync(long AddressId);
        Task RemoveAddress(long AddressId);
        Task<long> UpdateAddress(UserAddressEditable Address);
    }
}
