using SF.Biz.Delivery.DataModels;
using SF.Biz.Trades;
using SF.Common.Addresses.Management;
using SF.Sys;
using SF.Sys.Auth;
using SF.Sys.Data;
using SF.Sys.Entities;
using SF.Sys.Reminders;
using SF.Sys.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
namespace SF.Biz.Delivery.Management
{
    public class DeliveryManager:
        AutoModifiableEntityManager<ObjectKey<long>, DeliveryInternal, DeliveryInternal, DeliveryQueryArguments, DeliveryInternal, DataModels.DataDelivery>,
        IDeliveryManager,
        ITradeDeliveryProvider        
    {
        Lazy<IAddressSnapshotManager> AddressSnapshotManager { get; }
        Lazy<IUserAddressManager> UserAddressManager { get; }

        bool ITradeDeliveryProvider.DeliveryAddressRequired => true;

        public DeliveryManager(
            IEntityServiceContext ServiceContext, 
            Lazy<IAddressSnapshotManager> AddressSnapshotManager,
            Lazy<IUserAddressManager> UserAddressManager) : base(ServiceContext)
        {
            this.AddressSnapshotManager = AddressSnapshotManager;
            this.UserAddressManager = UserAddressManager;
        }

        protected virtual Task OnVerifyCreateArgument(DeliveryInternal editable)
        {
            Ensure.NotNull(editable.BizParent, "必须提供父业务");
            Ensure.NotNull(editable.BizRoot, "必须提供根业务");

            Ensure.HasContent(editable.Name, "必须提供发货标题");

            if (editable.Items == null || editable.Items.Count() == 0)
                throw new PublicArgumentException("没有发货内容");

            foreach (var it in editable.Items)
            {
                Ensure.HasContent(it.PayloadEntityIdent, "必须提供发货项内容ID");
                Ensure.Positive(it.Quantity, "必须提供发货项数量");
                Ensure.HasContent(it.Name, "必须提供发货项内容");
            }
            return Task.CompletedTask;

        }

        protected override async Task OnUpdateModel(IModifyContext ctx)
        {
            var model = ctx.Model;
            var editable = ctx.Editable;

            await OnVerifyCreateArgument(editable);
            await base.OnUpdateModel(ctx);
            model.TotalQuantity = model.Items.Sum(i => i.Quantity);
        }
        public async Task<TradeDeliveryResult> Create(TradeDeliveryCreateArgument arg)
        {

            var re = await CreateAsync(new DeliveryInternal
            {
                Name = arg.Name,
                BizParent = arg.BizParent,
                BizRoot = arg.BizRoot,
                DestAddressId = arg.DestAddressId,
                SourceAddressId = (await UserAddressManager.Value.QuerySingleEntityIdent(new UserAddressQueryArguments
                {
                    IsDefaultAddress = true,
                    OwnerId = arg.SenderId
                })).Id,
                ReceiverId = arg.ReceiverId,
                SenderId = arg.SenderId,
                Items = arg.Items.Select(i => new DeliveryItemInternal
                {
                    PayloadEntityIdent = i.PayloadEntityIdent,
                    Quantity = i.Quantity,
                    Name = i.Name,
                    Image = i.Image
                })
            });
            return new TradeDeliveryResult
            {
                DeliveryId = re.Id,
                State = TradeDeliveryState.WaitDeliverying
            };

        }

        public async Task Delivery(long DeliveryId)
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
                delivery.DeliveryOperatorId = ServiceContext.AccessToken.User.EnsureUserIdent();
                delivery.DestAddressSnapshotId = await AddressSnapshotManager.Value.CreateFromAddress(delivery.DestAddressId);
                delivery.SourceAddressSnapshotId = await AddressSnapshotManager.Value.CreateFromAddress(delivery.SourceAddressId);

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
                var remind = false;
                if (delivery.State == DeliveryState.Delivering)
                {
                    delivery.TransportId = Args.TransportId;
                    delivery.TransportCode = Args.TransportCode;
                    delivery.UpdatedTime = Time;
                    delivery.DeliveryOperatorId = ServiceContext.AccessToken.User.EnsureUserIdent();
                    //delivery.VirtualItemToken= VirtualItemToken;
                }
                else
                {
                    if (delivery.State != DeliveryState.CodeWaiting)
                        throw new InvalidOperationException("发货记录不是待发货状态");
                    remind = true;
                    delivery.State = DeliveryState.Delivering;
                    delivery.TransportId = Args.TransportId;
                    delivery.TransportCode = Args.TransportCode;
                    delivery.DeliveryTime =
                    delivery.UpdatedTime = Time;
                    delivery.DeliveryOperatorId = ServiceContext.AccessToken.User.EnsureUserIdent();

                }
                ctx.Update(delivery);

                if (remind)
                    ctx.AddCommitTracker(TransactionCommitNotifyType.AfterCommit, async (t, e) =>
                    {
                        if (e != null) return;
                        await ServiceContext.ServiceProvider.Resolve<IRemindService>().Remind(
                            delivery.BizRoot,
                            null,
                            true
                            );
                    });
                await ctx.SaveChangesAsync();
                var transportName = await ctx.Queryable<DataModels.DataDeliveryTransport>()
                    .Where(t => t.Id == delivery.TransportId.Value)
                    .Select(t => t.Name)
                    .SingleOrDefaultAsync();

                await ctx.EmitEvent(ServiceContext.EventEmitService, new DeliveryEvent(
                        delivery.Id,
                        delivery.ReceiverId.ToString(),
                        delivery.Name,
                        transportName,
                        delivery.TransportCode
                        ));
                

                return 0;
            });

        }

        public async Task Received(long DeliveryId)
        {
            await DataScope.Use("收货", async ctx =>
            {

                var delivery = await ctx.FindAsync<DataModels.DataDelivery>(DeliveryId);
                Ensure.NotNull(delivery, "发货记录不存在");
                if (delivery.State != DeliveryState.Delivering)
                    throw new InvalidOperationException("发货记录不是运输中");

                if (!delivery.ReceiverId.Equals(ServiceContext.AccessToken.User.EnsureUserIdent()))
                    throw new PublicDeniedException("操作人不是收货人");

                delivery.State = DeliveryState.Received;

                var Time = Now;


                delivery.ReceivedTime =
                delivery.UpdatedTime = Time;
                ctx.Update(delivery);
                ctx.AddCommitTracker(TransactionCommitNotifyType.AfterCommit, async (t, e) =>
                {
                    if (e != null) return;
                    await ServiceContext.ServiceProvider.Resolve<IRemindService>().Remind(
                        delivery.BizRoot,
                        null,
                        true
                        );
                });
                await ctx.SaveChangesAsync();
            });
        }

        public async Task Failed(DeliveryFailedArguments Arg)
        {
            await DataScope.Use("发货失败", async ctx =>
            {

                var delivery = await ctx.FindAsync<DataModels.DataDelivery>(Arg.DeliveryId);
                Ensure.NotNull(delivery, "发货记录不存在");
                if (delivery.State != DeliveryState.CodeWaiting && delivery.State!=DeliveryState.DeliveryWaiting)
                    throw new InvalidOperationException("已经发货，不能标记为失败");

                delivery.State = DeliveryState.Failed;
                delivery.Error = Arg.Error.Limit(200);
                var Time = Now;

                delivery.UpdatedTime = Time;
                ctx.Update(delivery);
                ctx.AddCommitTracker(TransactionCommitNotifyType.AfterCommit, async (t, e) =>
                {
                    if (e != null) return;
                    await ServiceContext.ServiceProvider.Resolve<IRemindService>().Remind(
                        delivery.BizRoot,
                        null,
                        true
                        );
                });
                await ctx.SaveChangesAsync();
            });
        }
        Task<TradeDeliveryResult> ITradeDeliveryProvider.QueryStatus(TrackIdent BizParent)
        {
            return DataScope.Use("查询发货地址", async ctx =>
            {
                var delivery= await ctx.Queryable<DataModels.DataDelivery>()
                    .Where(d => d.BizParent == BizParent.ToString())
                    .Select(d => new {d.Id, d.State,d.Error })
                    .SingleOrDefaultAsync();
                if (delivery == null)
                    throw new ArgumentException("找不到发货单:" + BizParent);
                return new TradeDeliveryResult
                {
                    State = delivery.State == DeliveryState.CodeWaiting ||
                            delivery.State == DeliveryState.DeliveryWaiting ? TradeDeliveryState.WaitDeliverying :
                            delivery.State == DeliveryState.Delivering ? TradeDeliveryState.WaitReceived :
                            delivery.State==DeliveryState.Received?TradeDeliveryState.Success:
                            TradeDeliveryState.Failed,
                    Error=delivery.Error,
                    DeliveryId = delivery.Id
                };                
            });
        }
    }

}
