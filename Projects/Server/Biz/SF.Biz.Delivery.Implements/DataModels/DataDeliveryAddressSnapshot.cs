using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SF.Sys.Data;
using SF.Sys.Entities.DataModels;

namespace SF.Biz.Delivery.DataModels
{
    /// <summary>
    /// 地址快照
    /// </summary>
    public class DataDeliveryAddressSnapshot : DataObjectEntityBase
	{
        /// <summary>
        /// 地址摘要(MD5)
        /// </summary>
        [Index(IsUnique =true)]
		[Required]
		[MaxLength(30)]
        public string Hash { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
		[Required]
		[MaxLength(30)]
        public string ContactName { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
		[Required]
		[MaxLength(30)]
        public string ContactPhoneNumber { get; set; }

        /// <summary>
        /// 地址位置ID
        /// </summary>
		[Index]
        public int LocationId { get; set; }

		[ForeignKey(nameof(LocationId))]
		public DataDeliveryLocation Location { get; set; }

        /// <summary>
        /// 地址位置名
        /// </summary>
		[Required]
		[MaxLength(200)]
        public string LocationName { get; set; }

        /// <summary>
        /// 邮编
        /// </summary>
		[Required]
		[MaxLength(20)]
        public string ZipCode { get; set; }

        /// <summary>
        /// 街道
        /// </summary>
		[Required]
		[MaxLength(200)]
        public string Address { get; set; }

	}
}
