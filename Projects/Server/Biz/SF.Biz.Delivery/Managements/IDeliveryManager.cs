using SF.Common.Addresses;
using SF.Sys.Annotations;
using SF.Sys.Auth;
using SF.Sys.Entities;
using SF.Sys.Entities.Models;
using SF.Sys.NetworkService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        /// 发货状态
        /// </summary>
        [TableVisible]
        public DeliveryState State { get; set; }

        [Ignore]
        public long SenderId { get; set; }

        /// <summary>
        /// 发货地址
        /// </summary>
        [EntityIdent(typeof(UserAddress))]
        public long SourceAddressId { get; set; }

        //[EntityIdent("DeliveryAddressSnapshop")]
        /// <summary>
        /// 收货地址
        /// </summary>
        [EntityIdent(typeof(UserAddress))]
        public long DestAddressId { get; set; }

        /// <summary>
        /// 发货地址快照
        /// </summary>
        [ReadOnly(true)]
        [EntityIdent(typeof(AddressSnapshot))]
        public long? SourceAddressSnapshotId { get; set; }

        //[EntityIdent("DeliveryAddressSnapshop")]
        /// <summary>
        /// 收货地址快照
        /// </summary>
        [ReadOnly(true)]
        [EntityIdent(typeof(AddressSnapshot))]
        public long? DestAddressSnapshotId { get; set; }

        /// <summary>
        /// 父业务
        /// </summary>
        [EntityIdent(WithBizType =true)]
        public string BizParent{ get; set; }

        /// <summary>
        /// 根业务
        /// </summary>
        [EntityIdent(WithBizType = true)]
        public string BizRoot { get; set; }

        [Ignore]
        public decimal Weight { get; set; }

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
		public AddressDetail DestAddress { get; set; }

    }
    public class DeliveryItemInternal : ObjectEntityBase
    {
        /// <summary>
        /// 发货
        /// </summary>
        [Ignore]
        [EntityIdent(typeof(DeliveryInternal))]
        public long DeliveryId { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        [Layout(1, 2)]
        [Uneditable]
        public int Quantity { get; set; }

        /// <summary>
        /// 项目内容
        /// </summary>
		[EntityIdent]
        [Uneditable]
        public string PayloadEntityIdent { get; set; }

        /// <summary>
        /// 图片
        /// </summary>
        [Image]
        [Uneditable]
        public string Image { get; set; }

        /// <summary>
        /// 失败原因
        /// </summary>
        [ReadOnly(true)]
        public string Error { get; set; }

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
        /// 父业务
        /// </summary>
		[EntityIdent]
        public string BizParent { get; set; }

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




    public class UpdateTransportCodeArguments
    {
        public long DeliveryId { get; set; }
        public long? TransportId { get; set; }
        public string TransportCode { get; set; }
    }
    public class DeliveryFailedArguments
    {
        public long DeliveryId { get; set; }
        public string Error { get; set; }
    }
    /// <summary>
    /// 发货管理器
    /// </summary>
    [NetworkService]
    [EntityManager]
    [DefaultAuthorize(PredefinedRoles.运营专员)]
    [DefaultAuthorize(PredefinedRoles.系统管理员)]

    public interface IDeliveryManager :
        IEntitySource<ObjectKey<long>, DeliveryInternal, DeliveryQueryArguments>
    {
        Task Delivery(long DeliveryId);
        Task UpdateTransportCode(UpdateTransportCodeArguments Args);
        Task Received(long DeliveryId);
        Task Failed(DeliveryFailedArguments Args);

    }


}
