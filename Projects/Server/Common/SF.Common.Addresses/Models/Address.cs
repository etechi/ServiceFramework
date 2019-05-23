using SF.Sys.Annotations;
using System.ComponentModel.DataAnnotations;

namespace SF.Common.Addresses
{
    public class AddressBase
	{
        /// <summary>
        /// 地址
        /// </summary>
        [StringLength(100)]
		[TableVisible(10)]
        [EntityTitle]
		public string Address { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        [TableVisible(30)]
		public string ContactName { get; set; }

        /// <summary>
        /// 联系人手机
        /// </summary>
        [TableVisible]
		public string ContactPhoneNumber { get; set; }

		[Ignore]
		public long LocationId { get; set; }
	}
	public class AddressDetail : AddressBase
	{
        /// <summary>
        /// 地区
        /// </summary>
        [TableVisible(20)]
		public string LocationName { get; set; }

		[Ignore]
		public string ZipCode { get; set; }
	}
}
