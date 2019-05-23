using SF.Sys.Data;
using SF.Sys.Entities.DataModels;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SF.Common.Addresses.DataModels
{
    /// <summary>
    /// 发货地址
    /// </summary>
    public class DataAddress: DataObjectEntityBase
	{

        /// <summary>
        /// 是否为默认地址
        /// </summary>
        public bool IsDefaultAddress { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
		[MaxLength(100)]
        public string ContactName { get; set; }

        /// <summary>
        /// 联系人电话
        /// </summary>
        [MaxLength(20)]
        public string ContactPhoneNumber { get; set; }

        /// <summary>
        /// 电话是否已确认
        /// </summary>
        public bool PhoneNumberVerified { get; set; }

        /// <summary>
        /// 地址位置ID
        /// </summary>
        [Index]
        public int ProvinceId { get; set; }

        /// <summary>
        /// 市
        /// </summary>
        [Index]
        public int CityId { get; set; }

        /// <summary>
        /// 县/区
        /// </summary>
        [Index]
        public int DistrictId { get; set; }

        [ForeignKey(nameof(ProvinceId))]
        public DataLocation Province { get; set; }

        [ForeignKey(nameof(CityId))]
        public DataLocation City { get; set; }

        [ForeignKey(nameof(DistrictId))]
		public DataLocation District { get; set; }

        /// <summary>
        /// 地址位置名
        /// </summary>
		[Required]
		[MaxLength(200)]
        public string LocationName { get; set; }

        /// <summary>
        /// 街道
        /// </summary>
		[Required]
		[MaxLength(200)]
        public string Address { get; set; }

        /// <summary>
        /// 邮编
        /// </summary>
		[MaxLength(20)]
        public string ZipCode { get; set; }

	}

}
