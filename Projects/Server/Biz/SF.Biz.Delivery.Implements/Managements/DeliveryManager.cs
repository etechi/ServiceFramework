using SF.Sys;
using SF.Sys.Auth;
using SF.Sys.Data;
using SF.Sys.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
namespace SF.Biz.Delivery.Management
{
    public class DeliveryManager:
        AutoModifiableEntityManager<ObjectKey<long>, DeliveryInternal, DeliveryInternal, DeliveryQueryArguments, DeliveryInternal, DataModels.DataDelivery>,
        IDeliveryManager
        
    {
        Lazy<IUserDeliveryAddressManager> UserDeliveryAddressManager { get; }
        public DeliveryManager(IEntityServiceContext ServiceContext, Lazy<IUserDeliveryAddressManager> UserDeliveryAddressManager) : base(ServiceContext)
        {
            this.UserDeliveryAddressManager = UserDeliveryAddressManager;
        }

        protected virtual Task OnVerifyCreateArgument(DeliveryCreateArgument arg)
        {
            Ensure.HasContent(arg.TrackEntityIdent, "必须提供业务来源ID");
            Ensure.HasContent(arg.Title, "必须提供发货标题");
            if (arg.Items == null || arg.Items.Length == 0)
                throw new PublicArgumentException("没有发货内容");

            foreach (var it in arg.Items)
            {
                Ensure.HasContent(it.PayloadEntityIdent, "必须提供发货项内容ID");
                Ensure.Positive(it.Quantity, "必须提供发货项数量");
                Ensure.HasContent(it.Name, "必须提供发货项内容");
            }
            return Task.CompletedTask;

        }
        public async Task<DeliveryCreateResult> Create(DeliveryCreateArgument arg)
         {
            await OnVerifyCreateArgument(arg);

            var time = Now;
            var did = await IdentGenerator.GenerateAsync<DataModels.DataDelivery>();
            var iids = await IdentGenerator.GenerateAsync<DataModels.DataDeliveryItem>(arg.Items.Length);

            var items = arg.Items.Select((i, idx) => new DataModels.DataDeliveryItem
            {
                Id= iids[idx],
                Name = i.Name.Limit(200),
                Order = idx,
                Quantity = i.Quantity,
                PayloadEntityIdent = i.PayloadEntityIdent,
                PayloadSpecEntityIdent = i.PayloadSpecEntityIdent,
                OwnerId=arg.ReceiverId,
                CreatedTime=time,
                DeliveryId=did
            }).ToArray();

            var delivery = new DataModels.DataDelivery
            {
                Id = did,
                OwnerId=arg.ReceiverId,

                TrackEntityIdent = arg.TrackEntityIdent,
                SellerId = arg.SenderId,
                BuyerId = arg.ReceiverId,
                State = DeliveryState.DeliveryWaiting,
                CreatedTime = time,
                UpdatedTime = time,
                DeliveryTime = null,
                ReceivedTime = null,

                SourceAddressSnapshotId = await UserDeliveryAddressManager.Value.GetSnapshot(arg.SourceAddressId),
                DestAddressSnapshotId = await UserDeliveryAddressManager.Value.GetSnapshot(arg.DestAddressId),
                TotalQuantity = items.Sum(i => i.Quantity),
                Items = items,
                TransportCode = null,
                TransportId = null,
                Name = arg.Title.Limit(200)
            };

            return await DataScope.Use("创建发货单", async ctx =>
            {
                ctx.Add(delivery);
                await ctx.SaveChangesAsync();
                return new DeliveryCreateResult
                {
                    DeliveryId = did,
                    DeliveryItemIds = iids
                };
            });
        }

        public async Task Delivery(long DeliveryId,long OperatorId)
        {
            await DataScope.Use("发货", async ctx =>
            {
                var delivery = await ctx.FindAsync<DataModels.DataDelivery>(DeliveryId);

                Ensure.NotNull(delivery, "发货记录不存在");
                var Time = Now;

                if (delivery.State != DeliveryState.DeliveryWaiting)
                    throw new InvalidOperationException("发货记录不是待发货状态");

                delivery.State = DeliveryState.CodeWaiting;
                delivery.UpdatedTime = Time;
                delivery.DeliveryOperatorId = OperatorId;

                ctx.Update(delivery);
                await ctx.SaveChangesAsync();

                return 0;
            });
            
        }
       
        public async Task UpdateTransportCode(UpdateTransportCodeArguments Args)
        {
            await DataScope.Use("填写单号", async ctx => {

                var delivery = await ctx.FindAsync<DataModels.DataDelivery>(Args.DeliveryId);
                Ensure.NotNull(delivery, "发货记录不存在");
                if (Args.TransportId == 0)
                        Args.TransportId = null;

                if (!Args.TransportId.HasValue|| string.IsNullOrWhiteSpace(Args.TransportCode))
                    throw new PublicArgumentException("请输入快递公司和快递单号");

                var Time = Now;
                if (delivery.State == DeliveryState.Delivering)
                {
                    delivery.TransportId = Args.TransportId;
                    delivery.TransportCode = Args.TransportCode;
                    delivery.UpdatedTime = Time;
                    delivery.DeliveryOperatorId = Args.OperatorId;
                    //delivery.VirtualItemToken= VirtualItemToken;
                }
                else
                {
                    if (delivery.State != DeliveryState.CodeWaiting)
                        throw new InvalidOperationException("发货记录不是待发货状态");

                    delivery.State = DeliveryState.Delivering;
                    delivery.TransportId = Args.TransportId;
                    delivery.TransportCode = Args.TransportCode;
                    delivery.DeliveryTime =
                    delivery.UpdatedTime = Time;
                    delivery.DeliveryOperatorId = Args.OperatorId;

                }
                ctx.Update(delivery);


                await ctx.SaveChangesAsync();
                var transportName = await ctx.Queryable<DataModels.DataDeliveryTransport>()
                    .Where(t => t.Id == delivery.TransportId.Value)
                    .Select(t => t.Name)
                    .SingleOrDefaultAsync();

                await ctx.EmitEvent(ServiceContext.EventEmitService, new DeliveryEvent(
                        delivery.Id,
                        delivery.BuyerId.ToString(),
                        delivery.Name,
                        transportName,
                        delivery.TransportCode
                        ));
                return 0;
            });

        }

        public async Task Received(long DeliveryId, long OperatorId)
        {
            await DataScope.Use("收货", async ctx =>
            {

                var delivery = await ctx.FindAsync<DataModels.DataDelivery>(DeliveryId);
                Ensure.NotNull(delivery, "发货记录不存在");
                if (delivery.State != DeliveryState.Delivering)
                    throw new InvalidOperationException("发货记录不是运输中");

                if (!delivery.BuyerId.Equals(OperatorId))
                    throw new PublicDeniedException("操作人不是收货人");

                delivery.State = DeliveryState.Received;

                var Time = Now;


                delivery.ReceivedTime =
                delivery.UpdatedTime = Time;
                ctx.Update(delivery);
                await ctx.SaveChangesAsync();
            });
        }
    }

}
