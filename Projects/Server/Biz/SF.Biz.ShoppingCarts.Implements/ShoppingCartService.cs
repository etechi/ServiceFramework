using SF.Biz.Products;
using SF.Sys;
using SF.Sys.Auth;
using SF.Sys.Clients;
using SF.Sys.Collections.Generic;
using SF.Sys.Data;
using SF.Sys.TimeServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace SF.Biz.ShoppingCarts
{
    public class ShoppingCartService:IShoppingCartService
    {
        public IDataScope DataScope { get; }
        public IItemService ItemService { get; }
        public ITimeService TimeService { get; }
        public IAccessToken AccessToken { get; }

        long EnsureUserIdent()
            => AccessToken.User.EnsureUserIdent();

        public ShoppingCartService(IItemService ItemService, ITimeService TimeService, IDataScope DataScope, IAccessToken AccessToken)
        {
            this.TimeService = TimeService;
            this.ItemService = ItemService;
            this.DataScope = DataScope;
            this.AccessToken = AccessToken;
        }

        string TypeNormalize(string Type)
        {
            if (string.IsNullOrEmpty(Type))
                return "main";
            return Type;
        }

        public async Task<int> Count( string Type)
        {
            long BuyerId = EnsureUserIdent();
            Type = TypeNormalize(Type);
            return await DataScope.Use("获取购物车项目", async Context =>
             {
                 return await Context.Queryable<DataModels.DataShoppingCartItem>()
                        .Where(m => m.BuyerId.Equals(BuyerId) && m.Type == Type).CountAsync();
             });

        }
        protected virtual IQueryable<ShoppingCartItem> MapModelToPublic(IQueryable<DataModels.DataShoppingCartItem> q)
        {
            return q.Select(m => new ShoppingCartItem
            {
                SellerId = m.SellerId,
                ProductId = m.ProductId,
                SkuId = m.SkuId,
                ItemId = m.ItemId,
                Title = m.Title,
                Image = m.Image,
                Spec = m.Spec,
                Quantity = m.Quantity,
                Selected = m.Selected,
            });
        }
        public async Task<ShoppingCartItem[]> List(QueryArguments Args)
        {
            var BuyerId = EnsureUserIdent();
            var Type = TypeNormalize(Args.Type);
            return await DataScope.Use("查询项目", async Context =>
             {
                 var mq = Context.Queryable<DataModels.DataShoppingCartItem>()
                         .Where(m => m.BuyerId.Equals(BuyerId) && m.Type == Type);
                 if (Args.Selected)
                     mq = mq.Where(i => i.Selected == true);

                 var q = MapModelToPublic(mq);

                 var results = await q.ToArrayAsync();

                 var pids = results.Select(i => i.ItemId).ToArray();
                 var items = (await ItemService.GetItems(pids)).ToDictionary(p => p.ItemId);
                 foreach (var result in results)
                 {
                     IItem item;
                     if (!items.TryGetValue(result.ItemId, out item))
                     {
                         result.LogicState = Sys.Entities.EntityLogicState.Disabled;
                         continue;
                     }
                     result.LogicState = !item.OnSale ? Sys.Entities.EntityLogicState.Disabled : Sys.Entities.EntityLogicState.Enabled;
                     result.CouponDisabled = item.CouponDisabled;
                     await OnUpdateResult(BuyerId, result, item, Args.IgnoreQuantityCheck);
                     if (!Args.IgnoreQuantityCheck)
                         result.Quantity = EnsureItemLimit(item, result.Quantity);
                 }
                 if (Args.Enabled)
                     return results.Where(t => t.LogicState == Sys.Entities.EntityLogicState.Enabled).ToArray();

                 if (!Args.IgnoreQuantityCheck)
                 {
                     foreach (var it in results)
                         if (it.LogicState != Sys.Entities.EntityLogicState.Enabled)
                             it.Selected = false;
                 }
                 return results;
             });
        }
        protected virtual Task OnUpdateResult(long BuyerId, ShoppingCartItem result, IItem item, bool IgnoreQuantityCheck)
        {
            result.Image = item.Image;
            result.Title = item.Title;
            result.SettlementPrice = item.Price;
            return Task.CompletedTask;
        }
        public Task<int> Add(ModifyArguments Args)
        {
            return UpdateInternal(
                Args.Type,
                Args.Items.Select(i => new ItemStatus { ItemId = i, Quantity = Args.Quantity, Selected = true }).ToArray(),
                (o, i) => o.Quantity + i.Quantity
                );

        }
        public Task<int> TryAdd(ModifyArguments Args)
        {
            return UpdateInternal(
                Args.Type,
                Args.Items.Select(i => new ItemStatus { ItemId = i, Quantity = Args.Quantity, Selected = true }).ToArray(),
                (o, i) => Math.Max(o.Quantity, i.Quantity)
                );

        }
        public Task<int> Update(ModifyArguments Args)
        {
            return UpdateInternal(
                Args.Type,
                Args.Items.Select(i => new ItemStatus { ItemId = i, Quantity = Args.Quantity }).ToArray(),
                (o, i) => i.Quantity
                );
        }
        public async Task Sync(SyncArguments Args)
        {
            await UpdateInternal(Args.Type, Args.Items, (o, i) => i.Quantity, true, Args.SkipLimitEnsure);
        }

        protected virtual void OnInitModel(DataModels.DataShoppingCartItem model, IItem item, ItemStatus status)
        {
            model.SkuId = 0;
            model.SellerId = item.SellerId;
            model.SellerTitle = null;
            model.ItemId = item.ItemId;
            model.ProductId = item.ProductId;
            model.Title = item.Title;
            model.Image = item.Image;
            model.Spec = null;
            model.MarketPrice = item.MarketPrice;
            model.Price = item.Price;
        }
        protected virtual void OnUpdateModel(DataModels.DataShoppingCartItem model, IItem item, ItemStatus status)
        {

        }
        async Task<int> UpdateInternal(            
            string Type,
            ItemStatus[] Items,
            Func<DataModels.DataShoppingCartItem, ItemStatus, int> QuantityUpdater,
            bool RemoveMissing = false,
            bool SkipLimitEnsure = false
            )
        {
            long BuyerId = EnsureUserIdent();
            Type = TypeNormalize(Type);
            foreach (var it in Items)
                Ensure.Range(it.Quantity, 0, 1000000, "数量不合法");


            if (Items == null)
                Items = Array.Empty<ItemStatus>();

            if (Items.Length == 0 && !RemoveMissing)
                return 0;

            return await DataScope.Retry("更新购物车项目",async Context =>{
                var added = 0;
                Type = "main";

                Dictionary<long, DataModels.DataShoppingCartItem> orgCartItems;
                if (RemoveMissing)
                    orgCartItems = await Context.Queryable<DataModels.DataShoppingCartItem>()
                        .Where(m => m.BuyerId.Equals(BuyerId) && m.Type == Type)
                        .ToDictionaryAsync(m => m.ItemId);
                else
                {
                    var ids = Items.Select(i => i.ItemId).ToArray();
                    orgCartItems = await Context.Queryable<DataModels.DataShoppingCartItem>()
                        .Where(m => m.BuyerId.Equals(BuyerId) && m.Type == Type && ids.Contains(m.ItemId))
                        .ToDictionaryAsync(m => m.ItemId);
                }
                if (RemoveMissing)
                {
                    var exists = Items.ToDictionary(i => i.ItemId);
                    var missing = orgCartItems.Values.Where(i => !exists.ContainsKey(i.ItemId)).ToArray();
                    Context.RemoveRange(missing);
                }

                var time = TimeService.Now;
                var newItems = new List<ItemStatus>();
                var productItems = (await ItemService.GetItems(Items.Select(i => i.ItemId).ToArray()))
                    .ToDictionary(p => p.ItemId);

                foreach (var it in Items)
                {
                    DataModels.DataShoppingCartItem item;
                    if (orgCartItems.TryGetValue(it.ItemId, out item))
                    {
                        var productItem = productItems.Get(it.ItemId);
                        if (item == null)
                            continue;
                        item.Quantity = SkipLimitEnsure ? QuantityUpdater(item, it) : EnsureItemLimit(productItem, QuantityUpdater(item, it));
                        item.UpdatedTime = time;
                        item.Selected = it.Selected;
                        OnUpdateModel(item, productItem, it);

                        Context.Update(item);
                    }
                    else
                        newItems.Add(it);
                }
                if (newItems.Count > 0)
                {
                    var nids = newItems.Select(i => i.ItemId).ToArray();
                    foreach (var it in newItems)
                    {
                        var item = productItems.Get(it.ItemId);
                        if (item == null)
                            continue;
                        var m = new DataModels.DataShoppingCartItem();
                        m.BuyerId = BuyerId;
                        m.Type = Type;
                        m.Quantity = SkipLimitEnsure ? it.Quantity : EnsureItemLimit(item, it.Quantity);
                        m.CreatedTime = time;
                        m.UpdatedTime = time;
                        m.Selected = it.Selected;
                        OnInitModel(m, item, it);
                        Context.Add(m);
                        added++;
                    }
                }
                await Context.SaveChangesAsync();
                return added;
            });
        }

        protected virtual int EnsureItemLimit(IItem item, int Quantity)
        {
            return Quantity;
        }

        public async Task Clear(string Type)
        {
            var BuyerId = EnsureUserIdent();
            Type = TypeNormalize(Type);
            await DataScope.Use("清除项目", async Context =>
            {
                var items = await Context.Queryable<DataModels.DataShoppingCartItem>()
                .Where(m => m.BuyerId.Equals(BuyerId) && m.Type == Type)
                .ToArrayAsync();
                Context.RemoveRange(items);
                await Context.SaveChangesAsync();
            });
        }
        public async Task<int> Remove(ModifyArguments Args)
        {
            var BuyerId = EnsureUserIdent();
            var Type = TypeNormalize(Args.Type);
            return await DataScope.Use("删除项目", async Context =>
            {
                var items = await Context.Queryable<DataModels.DataShoppingCartItem>()
                    .Where(m => m.BuyerId.Equals(BuyerId) && m.Type == Type && Args.Items.Contains(m.ItemId))
                    .ToArrayAsync();
                Context.RemoveRange(items);
                await Context.SaveChangesAsync();
                return items.Length;
            });
        }
    }
}
