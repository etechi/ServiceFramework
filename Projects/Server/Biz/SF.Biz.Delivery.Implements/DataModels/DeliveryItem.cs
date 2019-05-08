using ServiceProtocol.Annotations;
using ServiceProtocol.Biz.Delivery;
using ServiceProtocol.Data.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProtocol.Biz.Delivery.Entity.Models
{
	
	[Table("app_biz_delivery_item")]
    [TypeDisplay(GroupName = "发货服务", Name = "发货明细项")]
    public class DeliveryItem<TUserKey,TDelivery,TDeliveryItem,TAddress, TTransport,TLocation>
		where TUserKey: IEquatable<TUserKey>
		where TDelivery:Delivery<TUserKey,TDelivery,TDeliveryItem, TAddress, TTransport, TLocation>
		where TDeliveryItem: DeliveryItem<TUserKey, TDelivery, TDeliveryItem, TAddress, TTransport, TLocation>
		where TAddress : DeliveryAddressSnapshot
		where TTransport : DeliveryTransport
		where TLocation : DeliveryLocation
	{
		[Key]
        [Display(Name="ID")]
		public int Id { get; set; }

		[Required]
		[MaxLength(200)]
        [Display(Name = "标题")]
        public string Title { get; set; }

        [Display(Name = "排位")]
        public int Order { get; set; }

        [Display(Name = "数量")]
        public int Quantity { get; set; }

		
		[Required]
		[MaxLength(100)]
		[Index("payload")]
        [Display(Name = "负载业务跟踪标识")]
        public string PayloadEntityIdent { get; set; }

        [MaxLength(100)]
        [Index("payloadspec")]
        [Display(Name = "负载业务跟踪标识规格")]
        public string PayloadSpecEntityIdent { get; set; }

        [Display(Name = "发货ID")]
        public int DeliveryId { get; set; }

		[ForeignKey(nameof(DeliveryId))]
		public TDelivery Delivery { get; set; }

        [Display(Name = "是否为虚拟项目")]
        public bool VirtualItem { get; set; }

		[Display(Name = "虚拟项目自动发货规格")]
		public int? VIADSpecId { get; set; }

		[Display(Name = "虚拟项目自动发货记录")]
		public int? VIADDeliveryRecordId { get; set; }
	}
}
