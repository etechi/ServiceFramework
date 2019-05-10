using SF.Sys.Data;
using SF.Sys.Entities.DataModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SF.Biz.Delivery.DataModels
{

    /// <summary>
    /// 快递公司
    /// </summary>
    public class DataDeliveryTransport: DataUIObjectEntityBase
	{
        /// <summary>
        /// 排位
        /// </summary>
        [Index]
        public int Order { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
		[MaxLength(100)]
        public string ContactName { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
		[MaxLength(100)]        
        public string ContactPhone { get; set; }

        /// <summary>
        /// 代号
        /// </summary>
		[Required]
		[MaxLength(100)]
        public string Code { get; set; }

        /// <summary>
        /// 网站
        /// </summary>
		[MaxLength(100)]
        public string Site { get; set; }
	}
}
