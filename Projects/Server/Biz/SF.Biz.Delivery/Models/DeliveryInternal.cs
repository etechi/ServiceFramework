using SF.Sys.Annotations;
using SF.Sys.Auth;
using SF.Sys.Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SF.Biz.Delivery
{
    /// <summary>
    /// 发货
    /// </summary>
    [EntityObject]
    public class DeliveryInternal : ObjectEntityBase
	{
        /// <summary>
        /// 收货人
        /// </summary>
        [EntityIdent(typeof(User), nameof(ReceiverName))]
        public long ReceiverId { get; set; }

        /// <summary>
        /// 收货人
        /// </summary>
        [TableVisible]
        [Ignore]
        public string ReceiverName { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [TableVisible]
		public string Title { get; set; }


        /// <summary>
        /// 发货状态
        /// </summary>
        [TableVisible]
		public DeliveryState State { get; set; }

		[Ignore]
		public long SenderId { get; set; }
		//[EntityIdent("DeliveryAddressSnapshop")]
		[Ignore]
		public int? SourceAddressId { get; set; }

		//[EntityIdent("DeliveryAddressSnapshop")]
		[Ignore]
		public int? DestAddressId { get; set; }

        /// <summary>
        /// 业务来源
        /// </summary>
        [EntityIdent(null, nameof(Title))]
        public string TrackEntityIdent { get; set; }

		[Ignore]
		public int Weight { get; set; }

        /// <summary>
        /// 发货时间
        /// </summary>
		[TableVisible]
		[Layout(10, 2)]
		public DateTime? DeliveryTime { get; set; }


        /// <summary>
        /// 确认收货时间
        /// </summary>
        [TableVisible]
		[Layout(10, 3)]
		public DateTime? ReceivedTime { get; set; }

        /// <summary>
        /// 卡密读取时间
        /// </summary>
        [TableVisible]
        [Layout(10, 4)]
        public DateTime? VirtualItemTokenReadTime { get; set; }

        /// <summary>
        /// 总数量
        /// </summary>
        [TableVisible]
		public int TotalQuantity { get; set; }

        [Ignore]
        public int? TransportId { get; set; }

        /// <summary>
        /// 含虚拟商品
        /// </summary>
        public bool HasVirtualItem { get; set; }

        /// <summary>
        /// 含实物商品
        /// </summary>
        public bool HasObjectItem { get; set; }

        /// <summary>
        /// 快递公司
        /// </summary>
        public string TransportName { get; set; }
        /// <summary>
        /// 快递单号
        /// </summary>
        [Copyable]
        public string TransportCode { get; set; }

        ///<title>虚拟商品凭证</title>
        /// <summary>
        /// 虚拟商品凭证，如卡号/密码等
        /// </summary>
        [MultipleLines]
        public string VirtualItemData { get; set; }

        /// <summary>
        /// 发货明细
        /// </summary>
		public IEnumerable<DeliveryItemInternal> Items { get; set; }

        /// <summary>
        /// 收件地址
        /// </summary>
		public DeliveryAddressDetail DestAddress { get; set; }

    }
	public class DeliveryItemInternal
	{
		[Ignore]
		public int Id { get; set; }

		[Ignore]
		public int DeliveryId { get; set; }

        /// <summary>
        /// 项目
        /// </summary>
        [Layout(1,1)]
		public string Title { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        [Layout(1, 2)]
		public int Quantity { get; set; }

        /// <summary>
        /// 项目内容
        /// </summary>
		[EntityIdent(null,nameof(Title))]
		public string PayloadEntityIdent { get; set; }

        /// <summary>
        /// 项目内容规格
        /// </summary>
        [EntityIdent(null, nameof(PayloadEntityIdent))]
        public string PayloadSpecEntityIdent { get; set; }

        ///<title>虚拟项目</title>
        /// <summary>
        /// 没有实体，如卡密
        /// </summary>
        public bool VirtualItem { get; set; }
	}
}
