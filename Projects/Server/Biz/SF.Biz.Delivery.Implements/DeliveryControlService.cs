using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceProtocol.ObjectManager;
using ServiceProtocol.Data.Entity;
namespace ServiceProtocol.Biz.Delivery.Entity
{
	public class DeliveryControlService<TUserKey, TDelivery, TDeliveryItem, TAddress, TTransport,TLocation>:
		IVIADDeliverySupport<TUserKey>,
		IDeliveryControlService<TUserKey>
		where TUserKey : IEquatable<TUserKey>
        where TDelivery : Models.Delivery<TUserKey, TDelivery, TDeliveryItem, TAddress, TTransport, TLocation>
		where TDeliveryItem : Models.DeliveryItem<TUserKey, TDelivery, TDeliveryItem, TAddress,TTransport, TLocation>
		where TAddress : Models.DeliveryAddressSnapshot
		where TTransport : Models.DeliveryTransport
		where TLocation: Models.DeliveryLocation

    {
		public IDataContext Context { get; }
		public Times.ITimeService TimeService { get; }
        public Lazy<ServiceProtocol.Events.IEventEmitter> EventEmitter { get; }
        public IAuditService AuditService { get; }
        public DeliveryControlService(
            IDataContext Context, 
            Times.ITimeService TimeService,
            Lazy<ServiceProtocol.Events.IEventEmitter> EventEmitter,
            IAuditService AuditService
            )
        {
			this.Context = Context;
			this.TimeService = TimeService;
            this.EventEmitter = EventEmitter;
            this.AuditService = AuditService;

        }
        public Task Delivery(int DeliveryId, TUserKey OperatorId)
        {
            return AuditService.Record(async (ac) =>
            {
                var delivery = await Context.Editable<TDelivery>().FindAsync(DeliveryId);
                
                ac.DestId = "发货-" + delivery.Id;
                ac.Operation = "备货确认";
                ac.Resource = "发货";
                ac.OwnerId = delivery.BuyerId.ToString();
                ac.SetOrgState(delivery);

                Ensure.NotNull(delivery, "发货记录不存在");
                var Time = TimeService.Now;

                if (delivery.State != DeliveryState.DeliveryWaiting)
                    throw new InvalidOperationException("发货记录不是待发货状态");

                delivery.State = DeliveryState.CodeWaiting;
                delivery.UpdatedTime = Time;
                delivery.DeliveryOperatorId = OperatorId;
                
                Context.Update(delivery);
                await Context.SaveChangesAsync();
                ac.SetNewState(delivery);

                return 0;
            });
        }
		async Task SendDeliveryEvent(TDelivery delivery)
		{
			var transportName = delivery.TransportId.HasValue ? await Context
				.ReadOnly<TTransport>()
				.Where(t => t.Id == delivery.TransportId.Value)
				.Select(t => t.Name)
				.SingleOrDefaultAsync() : null;

			await EventEmitter.Value.Emit(new DeliveryEvent(
					delivery.Id,
					delivery.BuyerId.ToString(),
					delivery.Title,
					transportName,
					delivery.TransportCode,
					delivery.HasObjectItem,
					delivery.HasVirtualItem
					),
					false);
		}
		public async Task UpdateTransportCode(int DeliveryId, TUserKey OperatorId, int? TransportId, string TransportCode,string VirtualItemToken)
		{
            var delivery = await Context.Editable<TDelivery>().FindAsync(DeliveryId);
            Ensure.NotNull(delivery, "发货记录不存在");
            if (TransportId == 0)
                TransportId = null;

            if (delivery.HasObjectItem && (TransportId == null || string.IsNullOrWhiteSpace(TransportCode)))
                throw new PublicArgumentException("请输入快递公司和快递单号");
            if(delivery.HasVirtualItem && string.IsNullOrWhiteSpace(VirtualItemToken))
                throw new PublicArgumentException("请输入虚拟商品卡密");

            await AuditService.Record(async (ac) =>
            {
                ac.DestId = "发货-" + delivery.Id;
                ac.Operation = "发货";
                ac.Resource = "发货";
                ac.OwnerId = delivery.BuyerId.ToString();
                ac.SetOrgState(delivery);
                
                var Time = TimeService.Now;
                if (delivery.State == DeliveryState.Delivering)
                {
                    delivery.TransportId = TransportId;
                    delivery.TransportCode = TransportCode;
                    delivery.UpdatedTime = Time;
                    delivery.DeliveryOperatorId = OperatorId;
                    //delivery.VirtualItemToken= VirtualItemToken;
                }
                else
                {
                    if (delivery.State != DeliveryState.CodeWaiting)
                        throw new InvalidOperationException("发货记录不是待发货状态");

                    delivery.State = DeliveryState.Delivering;
                    delivery.TransportId = TransportId;
                    delivery.TransportCode = TransportCode;
                    delivery.VirtualItemToken = VirtualItemToken;
                    delivery.DeliveryTime =
                    delivery.UpdatedTime = Time;
                    delivery.DeliveryOperatorId = OperatorId;
                    
                }
                Context.Update(delivery);


                await Context.SaveChangesAsync();
                ac.SetNewState(delivery);
				await SendDeliveryEvent(delivery);
                return 0;
            });

        }

        public async Task Received(int DeliveryId,TUserKey OperatorId)
		{
			var delivery = await Context.Editable<TDelivery>().FindAsync(DeliveryId);
			Ensure.NotNull(delivery, "发货记录不存在");
			if (delivery.State != DeliveryState.Delivering)
				throw new InvalidOperationException("发货记录不是运输中");

			if (!delivery.BuyerId.Equals(OperatorId))
				throw new PublicDeniedException("操作人不是收货人");

            if(!delivery.HasVirtualItem || delivery.VirtualItemTokenReadTime.HasValue)
			    delivery.State = DeliveryState.Received;

			var Time = TimeService.Now;

			delivery.ReceivedTime =
			delivery.UpdatedTime = Time;
			Context.Update(delivery);
			await Context.SaveChangesAsync();
		}
        public async Task<string> GetVirtualItemToken(int DeliveryId, TUserKey OperatorId)
        {
            var delivery = await Context.Editable<TDelivery>().FindAsync(DeliveryId);
            Ensure.NotNull(delivery, "发货记录不存在");
            if (delivery.State != DeliveryState.Delivering && 
                delivery.State != DeliveryState.Received 
                )
                throw new InvalidOperationException("发货记录不是运输中");

            if (!delivery.BuyerId.Equals(OperatorId))
                throw new PublicDeniedException("操作人不是收货人");

            if(!delivery.HasVirtualItem)
                throw new PublicInvalidOperationException("没有虚拟商品");

            if (string.IsNullOrWhiteSpace(delivery.VirtualItemToken))
                throw new PublicInvalidOperationException("没有设置虚拟商品卡密");

            if (delivery.VirtualItemTokenReadTime == null)
            {
                if (!delivery.HasObjectItem || delivery.ReceivedTime.HasValue)
                    delivery.State = DeliveryState.Received;

                var Time = TimeService.Now;

                delivery.VirtualItemTokenReadTime =
                delivery.UpdatedTime = Time;
                Context.Update(delivery);
                await Context.SaveChangesAsync();
            }
            return delivery.VirtualItemToken;
        }

		async Task<VIADDelivery<TUserKey>[]> IVIADDeliverySupport<TUserKey>.Query(int Limit)
		{
			var re = await Context.ReadOnly<TDelivery>()
				.Where(d =>
					d.State == DeliveryState.CodeWaiting &&
					!d.HasObjectItem &&
					d.HasVirtualItem &&
					d.Items.All(i => i.VIADSpecId.HasValue && i.Quantity==1)
					)
				.OrderBy(d => d.CreatedTime)
				.Select(d => new VIADDelivery<TUserKey>
				{
					Id=d.Id,
					UserId=d.BuyerId,
					Items=d.Items.Select(i=>new VIADDeliveryItem {
						Id=i.Id,
						PayloadId=i.PayloadEntityIdent,
						PayloadSpecId=i.PayloadSpecEntityIdent,
						Title=i.Title,
						VIADSpecId=i.VIADSpecId.Value
					})
				}).ToArrayAsync();
			return re;
		}

		
		async Task IVIADDeliverySupport<TUserKey>.SendEvent(int DeliveryId)
		{
			var delivery = await Context.Editable<TDelivery>()
				   .Where(d => d.Id == DeliveryId)
				   .SingleOrDefaultAsync();
			if (delivery == null)
				return;
			await SendDeliveryEvent(delivery);
		}
		async Task IVIADDeliverySupport<TUserKey>.AutoDeliveryCompleted(
			int DeliveryId, 
			Dictionary<int, Tuple<int,string>> Records,
			DateTime Time
			)
		{
			var delivery = await Context.Editable<TDelivery>()
				.Where(d => d.Id == DeliveryId)
				.Include(d => d.Items)
				.SingleOrDefaultAsync();
			if (delivery == null)
				throw new InvalidOperationException("找不到发货记录:" + DeliveryId);
			if (delivery.State != DeliveryState.CodeWaiting)
				throw new InvalidOperationException("发货记录不是待发货状态");
			if(delivery.HasObjectItem)
				throw new InvalidOperationException("发货记录包含实物，不能自动发货");
			if (!delivery.HasVirtualItem)
				throw new InvalidOperationException("发货记录未包含虚拟物品，不能自动发货");

			var count = delivery.Items.Count;
			var sb = new StringBuilder();
			foreach (var it in delivery.Items)
			{
				var re = Records.Get(it.Id);
				if (re == null)
					throw new InvalidOperationException($"找不到发货条目对应的自动发货记录ID:发货:{DeliveryId} 发货项目:{it.Id}");
				it.VIADDeliveryRecordId = re.Item1;
				if (count > 1)
					sb.AppendLine(it.Title + ":");
				sb.AppendLine(re.Item2);
				sb.AppendLine();

				Context.Update(it);
			}
			delivery.AutoDeliveried = true;
			delivery.State = DeliveryState.Delivering;
			delivery.VirtualItemToken = sb.ToString();
			delivery.DeliveryTime =
			delivery.UpdatedTime = Time;
			delivery.DeliveryOperatorId = default(TUserKey);
			Context.Update(delivery);
			await Context.SaveChangesAsync();
		}
	}
}
