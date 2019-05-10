using SF.Sys.Data;
using SF.Sys.Entities.DataModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SF.Biz.Delivery.DataModels
{

    /// <summary>
    /// 发货明细项
    /// </summary>
    public class DataDeliveryItem : DataObjectEntityBase
	{

        /// <summary>
        /// 排位
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 负载业务跟踪标识
        /// </summary>
        [Required]
		[MaxLength(100)]
		[Index("payload")]
        public string PayloadEntityIdent { get; set; }

        /// <summary>
        /// 负载业务跟踪标识规格
        /// </summary>
        [MaxLength(100)]
        [Index("payloadspec")]
        public string PayloadSpecEntityIdent { get; set; }

        /// <summary>
        /// 发货ID
        /// </summary>
        public long DeliveryId { get; set; }

		[ForeignKey(nameof(DeliveryId))]
		public DataDelivery Delivery { get; set; }

	}
}
