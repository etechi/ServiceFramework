using SF.Sys.Data;
using SF.Sys.Entities.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SF.Biz.Delivery.DataModels
{


    /// <summary>
    /// 发货
    /// </summary>
	public class DataDelivery : DataObjectEntityBase
	{
        /// <summary>
        /// 根业务ID
        /// </summary>
		[Index]
        [Required]
        [MaxLength(100)]
        public string BizRoot { get; set; }

        /// <summary>
        /// 业务跟踪ID
        /// </summary>
		[Index("biz-ident",IsUnique =true)]
		[Required]
		[MaxLength(100)]
        public string BizParent { get; set; }

        /// <summary>
        /// 发货人
        /// </summary>
        [Index("seller",Order =1)]
        public long SenderId { get; set; }

        /// <summary>
        /// 收货人ID
        /// </summary>
		[Index("buyer", Order = 1)]
        public long ReceiverId { get; set; }

        /// <summary>
        /// 发货状态
        /// </summary>
		[Index("seller", Order = 2)]
		[Index("buyer", Order = 2)]
        public DeliveryState State { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
		[Index("buyer", Order = 3)]
		[Index("seller", Order = 3)]
		public override DateTime CreatedTime { get; set; }


        /// <summary>
        /// 发货时间
        /// </summary>
        public DateTime? DeliveryTime { get; set; }

        /// <summary>
        /// 签收时间
        /// </summary>
        public DateTime? ReceivedTime { get; set; }

        /// <summary>
        /// 发货地址ID
        /// </summary>
        public long SourceAddressId { get; set; }

        /// <summary>
        /// 收货地址ID
        /// </summary>
        public long DestAddressId { get; set; }

		

        /// <summary>
        /// 发货地址快照ID
        /// </summary>
        public long? SourceAddressSnapshotId { get; set; }

        /// <summary>
        /// 收货地址快照ID
        /// </summary>
        public long? DestAddressSnapshotId { get; set; }

        


        [InverseProperty(nameof(DataDeliveryItem.Delivery))]
		public ICollection<DataDeliveryItem> Items { get; set; }


        /// <summary>
        /// 总数量
        /// </summary>
        public int TotalQuantity { get; set; }

        /// <summary>
        /// 运单号
        /// </summary>
		[MaxLength(50)]        
        public string TransportCode { get; set; }

        /// <summary>
        /// 物流公司ID
        /// </summary>
        public long? TransportId { get; set; }

		[ForeignKey(nameof(TransportId))]
		public DataDeliveryTransport Transport { get; set; }

        /// <summary>
        /// 发货操作人ID
        /// </summary>
        [Index]
		public long DeliveryOperatorId { get; set; }

        [MaxLength(200)]
        public string Error { get; set; }

	}
}
