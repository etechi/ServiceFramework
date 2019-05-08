using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceProtocol.ObjectManager;
using ServiceProtocol.Data.Entity;
namespace ServiceProtocol.Biz.Delivery.Entity
{
	public class DeliveryCreateService<TUserKey, TDelivery, TDeliveryItem, TAddress, TTransport,TLocation>:
		IDeliveryCreateService<TUserKey> 
		where TUserKey : IEquatable<TUserKey>
		where TDelivery : Models.Delivery<TUserKey, TDelivery, TDeliveryItem, TAddress, TTransport, TLocation>,new()
		where TDeliveryItem : Models.DeliveryItem<TUserKey, TDelivery, TDeliveryItem, TAddress, TTransport, TLocation>,new()
		where TAddress : Models.DeliveryAddressSnapshot
		where TTransport : Models.DeliveryTransport
		where TLocation: Models.DeliveryLocation
	{
		public IDataContext Context { get; }
		public Times.ITimeService TimeService { get; }
		public DeliveryCreateService(IDataContext Context, Times.ITimeService TimeService)
		{
			this.Context = Context;
			this.TimeService = TimeService;
		}

		protected virtual Task OnVerifyCreateArgument(DeliveryCreateArgument<TUserKey> arg)
		{
			Ensure.HasContent(arg.TrackEntityIdent, "必须提供业务来源ID");
			Ensure.HasContent(arg.Title, "必须提供发货标题");
			if (arg.Items == null || arg.Items.Length == 0)
				throw new ArgumentException("没有发货内容");

			foreach(var it in arg.Items)
			{
				Ensure.HasContent(it.PayloadEntityIdent, "必须提供发货项内容ID");
				Ensure.Positive(it.Quantity, "必须提供发货项数量");
				Ensure.HasContent(it.Title, "必须提供发货项内容");
			}
			return Task.CompletedTask;

		}
		public async Task<DeliveryCreateResult> Create(DeliveryCreateArgument<TUserKey> arg)
		{
			await OnVerifyCreateArgument(arg);

			var time = TimeService.Now;

			var items = arg.Items.Select((i, idx) => new TDeliveryItem
			{
				Title = i.Title.Limit(200),
				Order = idx,
				Quantity = i.Quantity,
				PayloadEntityIdent = i.PayloadEntityIdent,
                PayloadSpecEntityIdent = i.PayloadSpecEntityIdent,
                VirtualItem =i.VirtualItem,
				VIADSpecId=i.VIADSpecId
			}).ToArray();

            var delivery = new TDelivery
            {
                TrackEntityIdent = arg.TrackEntityIdent,
                SellerId = arg.SenderId,
                BuyerId = arg.ReceiverId,
                State = DeliveryState.DeliveryWaiting,
                CreatedTime = time,
                UpdatedTime = time,
                DeliveryTime = null,
                ReceivedTime = null,

                SourceAddressSnapshotId = arg.SourceAddressId,
                DestAddressSnapshotId = arg.DestAddressId,
                TotalQuantity = items.Sum(i => i.Quantity),
                Items = items,
                TransportCode = null,
                TransportId = null,
                Title = arg.Title.Limit(200),
                HasVirtualItem = items.Any(i => i.VirtualItem),
                HasObjectItem = items.Any(i => !i.VirtualItem)
            };

            if (delivery.HasObjectItem && (!delivery.SourceAddressSnapshotId.HasValue || !delivery.DestAddressSnapshotId.HasValue))
                throw new PublicArgumentException("发货单中包含实物，需要提供地址");

            if (delivery.HasVirtualItem && !delivery.HasObjectItem)
                delivery.State = DeliveryState.CodeWaiting;

			Context.Add(delivery);
			await Context.SaveChangesAsync();
            return new DeliveryCreateResult
            {
                DeliveryId = delivery.Id,
                DeliveryItemIds = delivery.Items.Select(i => i.Id).ToArray()
            };
		}
	}
}
