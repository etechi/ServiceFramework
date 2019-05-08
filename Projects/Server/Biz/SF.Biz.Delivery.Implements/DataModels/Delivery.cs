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
	

	[Table("app_biz_delivery")]
    [TypeDisplay(GroupName="发货服务",Name="发货")]
	public class Delivery<TUserKey, TDelivery, TDeliveryItem,TAddress,TTransport,TLocation>
		where TUserKey : IEquatable<TUserKey>
		where TDelivery : Delivery<TUserKey, TDelivery, TDeliveryItem, TAddress, TTransport,TLocation>
		where TDeliveryItem : DeliveryItem<TUserKey, TDelivery, TDeliveryItem, TAddress, TTransport,TLocation>
		where TAddress: DeliveryAddressSnapshot
		where TTransport : DeliveryTransport
		where TLocation : DeliveryLocation
	{
		[Key]
        [Display(Name="ID")]
		public int Id { get; set; }

		[Index("biz-ident",IsUnique =true)]
		[Required]
		[MaxLength(100)]
        [Display(Name = "业务跟踪ID")]
        public string TrackEntityIdent { get; set; }

        [Index("seller",Order =1)]
        [Display(Name = "卖家ID")]
        public TUserKey SellerId { get; set; }

		[Index("buyer", Order = 1)]
        [Display(Name = "买家ID")]
        public TUserKey BuyerId { get; set; }

		[Index("seller", Order = 2)]
		[Index("buyer", Order = 2)]
        [Display(Name = "发货状态")]
		[Index("VIAD",Order =1)]
        public DeliveryState State { get; set; }

		[Index("buyer", Order = 3)]
		[Index("seller", Order = 3)]
        [Display(Name = "创建时间")]
		[Index("VIAD", Order = 3)]
		public DateTime CreatedTime { get; set; }

		[Display(Name = "更新时间")]
        public DateTime UpdatedTime { get; set; }

		[Display(Name = "发货时间")]
		public DateTime? DeliveryTime { get; set; }

        [Display(Name = "签收时间")]
        public DateTime? ReceivedTime { get; set; }

        [Display(Name = "发货地址快照ID",Description ="虚拟商品可以没有地址")]
        public int? SourceAddressSnapshotId { get; set; }
        [Display(Name = "收货地址快照ID", Description = "虚拟商品可以没有地址")]
        public int? DestAddressSnapshotId { get; set; }

		[ForeignKey(nameof(SourceAddressSnapshotId))]
		public TAddress SourceAddressSnapshot { get; set; }

		[ForeignKey(nameof(DestAddressSnapshotId))]
		public TAddress DestAddressSnapshot { get; set; }

		[InverseProperty(nameof(DeliveryItem< TUserKey, TDelivery, TDeliveryItem,TAddress, TTransport,TLocation>.Delivery))]
		public ICollection<TDeliveryItem> Items { get; set; }

		[Required]
		[MaxLength(200)]
        [Display(Name = "标题")]
        public string Title { get; set; }

        [Display(Name = "总数量")]
        public int TotalQuantity { get; set; }

		[MaxLength(50)]
        [Display(Name = "运单号")]
        public string TransportCode { get; set; }

        [Display(Name = "物流公司ID")]
        public int? TransportId { get; set; }

		[ForeignKey(nameof(TransportId))]
		public TTransport Transport { get; set; }

        [Display(Name = "卡密密钥")]
        public string VirtualItemToken { get; set; }

        [Display(Name = "是否有虚拟物品")]
		[Index("VIAD", Order = 2)]
		public bool HasVirtualItem { get; set; }

        [Display(Name = "是否有实物物品")]
        public bool HasObjectItem { get; set; }

        [Display(Name = "卡密密钥读取时间")]
        public DateTime? VirtualItemTokenReadTime { get; set; }

        [Display(Name = "发货操作人ID")]
        [Index]
		public TUserKey DeliveryOperatorId { get; set; }

		[Display(Name = "是否为自动发货")]
		public bool AutoDeliveried { get; set; }
	}
}
