﻿using SF.Sys.Annotations;
using SF.Sys.Auth;
using SF.Sys.Entities.Models;
using SF.Sys.NetworkService;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace SF.Common.Addresses.Management
{
    /// <summary>
    /// 发货地址
    /// </summary>
    [EntityObject]
    public class UserAddressInternal : ObjectEntityBase
    {
        /// <summary>
        /// 用户
        /// </summary>
        [EntityIdent(typeof(User),nameof(UserName))]
        public long? OwnerId { get; set; }

        /// <summary>
        /// 用户
        /// </summary>
        [TableVisible]
        [Ignore]
        public string UserName { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        [Required]
        [MaxLength(50)]
        [TableVisible]
        public string ContactName { get; set; }

        /// <summary>
        /// 联系人手机
        /// </summary>
        [Required]
        [MaxLength(50)]
        [TableVisible]
        public string ContactPhoneNumber { get; set; }

        /// <summary>
        /// 手机是否验证
        /// </summary>
        [TableVisible]
        public bool PhoneNumberVerified { get; set; }

        /// <summary>
        /// 省
        /// </summary>
        [EntityIdent(typeof(LocationInternal))]
        public int ProvinceId { get; set; }

        /// <summary>
        /// 市
        /// </summary>
        [EntityIdent(typeof(LocationInternal), ScopeField = nameof(ProvinceId))]
        public int CityId { get; set; }

        /// <summary>
        /// 县/区
        /// </summary>
        [EntityIdent(typeof(LocationInternal), ScopeField = nameof(CityId))]
        public int DistrictId { get; set; }

        [FromEntityProperty(nameof(DistrictId),nameof(LocationInternal.FullName))]
        [TableVisible]
        [Ignore]
        [ReadOnly(true)]
        public string LocationName { get; set; }

        /// <summary>
        /// 默认地址
        /// </summary>
        [TableVisible]
        public bool IsDefaultAddress { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        [StringLength(100)]
        public string Address { get; set; }

    }
}
