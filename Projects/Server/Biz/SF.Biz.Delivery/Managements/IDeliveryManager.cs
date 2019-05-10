using SF.Sys.Annotations;
using SF.Sys.Auth;
using SF.Sys.Entities;
using SF.Sys.Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace SF.Biz.Delivery.Management
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
        public long SourceAddressId { get; set; }

        //[EntityIdent("DeliveryAddressSnapshop")]
        [Ignore]
        public long DestAddressId { get; set; }

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


        /// <summary>
        /// 快递公司
        /// </summary>
        [EntityIdent(typeof(DeliveryTransport),nameof(TransportName))]
        public long TransportId { get; set; }

        /// <summary>
        /// 快递公司
        /// </summary>
        [TableVisible]
        [Ignore]
        public string TransportName { get; set; }

        /// <summary>
        /// 快递单号
        /// </summary>
        [Copyable]
        public string TransportCode { get; set; }


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
        public long Id { get; set; }

        [Ignore]
        public long DeliveryId { get; set; }

        /// <summary>
        /// 项目
        /// </summary>
        [Layout(1, 1)]
        public string Title { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        [Layout(1, 2)]
        public int Quantity { get; set; }

        /// <summary>
        /// 项目内容
        /// </summary>
		[EntityIdent(null, nameof(Title))]
        public string PayloadEntityIdent { get; set; }

        /// <summary>
        /// 项目内容规格
        /// </summary>
        [EntityIdent(null, nameof(PayloadEntityIdent))]
        public string PayloadSpecEntityIdent { get; set; }

    }
    public class DeliveryQueryArguments:QueryArgument
	{
        /// <summary>
        /// 收件人
        /// </summary>
		[EntityIdent(typeof(User))]
		[Ignore]
		public long ReceiverId { get; set; }

        /// <summary>
        /// 发货状态
        /// </summary>
        public DeliveryState? State { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateQueryRange CreateTime { get; set; }
        
        /// <summary>
        /// 标题
        /// </summary>
        [StringLength(100)]
		public string Search { get; set; }
	}



    public class DeliveryCreateArgument
    {
        public string Title { get; set; }
        public long SenderId { get; set; }
        public long ReceiverId { get; set; }

        public long SourceAddressId { get; set; }
        public long DestAddressId { get; set; }
        public string TrackEntityIdent { get; set; }
        public long TotalQuantity { get; set; }
        public long Weight { get; set; }
        public DeliveryItemCreateArgument[] Items { get; set; }
    }
    public class DeliveryItemCreateArgument
    {
        public string Name { get; set; }
        public string PayloadEntityIdent { get; set; }
        public string PayloadSpecEntityIdent { get; set; }
        public int Quantity { get; set; }
        public bool VirtualItem { get; set; }
    }
    public class DeliveryCreateResult
    {
        public long DeliveryId { get; set; }
        public long[] DeliveryItemIds { get; set; }
    }

    public class UpdateTransportCodeArguments
    {
        public long DeliveryId { get; set; }
        public long OperatorId { get; set; }
        public long? TransportId { get; set; }
        public string TransportCode { get; set; }
    }

    public interface IDeliveryManager :
        IEntitySource<ObjectKey<long>, DeliveryInternal, DeliveryQueryArguments>
    {

        Task<DeliveryCreateResult> Create(DeliveryCreateArgument arg);
        Task Delivery(long DeliveryId, long OperatorId);
        Task UpdateTransportCode(UpdateTransportCodeArguments Args);
        Task Received(long DeliveryId, long OperatorId);

    }


}
