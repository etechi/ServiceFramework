using SF.Biz.Trades.Managements;
using SF.Biz.Trades.StateProviders;
using SF.Sys.Auth;
using SF.Sys.Clients;
using SF.Sys.Data;
using SF.Sys.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Biz.Trades
{
    public class SellerTradeService:
        ISellerTradeService
	{
		public IDataScope DataScope { get; }
        public IAccessToken AccessToken { get; }
        public Lazy<ITradeManager> TradeManager { get; }
        long EnsureUserIdent() => AccessToken.User.EnsureUserIdent();
        public SellerTradeService(IDataScope DataScope, IAccessToken AccessToken, Lazy<ITradeManager> TradeManager)
        {
			this.DataScope= DataScope;
            this.AccessToken = AccessToken;
            this.TradeManager = TradeManager;
        }
		protected virtual IQueryable<Trade> MapModelToPublic(IQueryable<DataModels.DataTrade> query)
		{
			return from t in query
				   select new Trade
				   {
					   Id = t.Id,
					   CreatedTime = t.CreatedTime,
					   Image = t.Image,
					   Name = t.Name,
					   SellerId = t.SellerId,
					   BuyerId = t.BuyerId,
					   Amount = t.TotalAmount,
					   DiscountDesc = t.DiscountDesc,
                       DiscountEntityId = t.DiscountEntityIdent,
                       DiscountEntityCount = t.DiscountEntityCount,
                       SettlementAmount = t.TotalSettlementAmount,
					   State = t.State,
					   EndType = t.EndType,
                       EndReason=t.EndReason,
					   LastStateTime = t.LastStateTime,
					   BuyerRemarks = t.BuyerRemarks
				   };
		}
		protected virtual IQueryable<TradeItem> MapItemModelToPublic(IQueryable<DataModels.DataTradeItem> query)
		{
			return from i in query
				   select new TradeItem
				   {
					   Id = i.Id,
					   Image = i.Image,
					   Name= i.Name,
					   ProductId = i.ProductId,
					   Price = i.Price,
					   Quantity = i.Quantity,
					   Amount = i.Amount,
                       
                       AmountAfterDiscount=i.AmountAfterDiscount,
                       DiscountEntityIdent = i.DiscountEntityIdent,
                       SettlementAmount = i.SettlementAmount,
					   SellerRemarks = i.SellerRemarks,
					   BuyerRemarks = i.BuyerRemarks
				   };
		}
		public async Task<Trade> Get(long tradeId, bool withItems)
        {
            var uid = EnsureUserIdent();
            return await DataScope.Use("获取订单信息", async ctx =>
            {
                var q = MapModelToPublic(
                    ctx.Queryable<DataModels.DataTrade>()
                    .Where(id => id.Equals(tradeId))
                    );
                var re = await q.SingleOrDefaultAsync();
                if (re == null || re.SellerId != uid)
                    return null;
                if (withItems)
                    re.Items = await MapItemModelToPublic(
                            ctx.Queryable<DataModels.DataTradeItem>().Where(ti => ti.TradeId.Equals(tradeId)).OrderBy(ti => ti.Order)
                        ).ToArrayAsync();

                return re;
            });
		}

		static readonly PagingQueryBuilder<DataModels.DataTrade> PagingBuilder = new PagingQueryBuilder<DataModels.DataTrade>(
			"create",
			i => i.Add("create", m => m.CreatedTime, true)
			);
		public async Task<QueryResult<Trade>> Query(SellerTradeQueryArgument Arg)
        {
            var uid = EnsureUserIdent();

            return await DataScope.Use("查询卖家订单", async ctx =>
            {
                return await ctx.Queryable<DataModels.DataTrade>()
                    .Where(t => t.BuyerId.Equals(uid))
                    .ToQueryResultAsync(
                        MapModelToPublic,
                        r => r,
                        PagingBuilder,
                        Arg.Paging
                        );
            });

		}

        public async Task Delivery(long tradeId)
        {
            await TradeManager.Value.Advance(tradeId, TradeState.SellerComplete, new SellerCompleteArgument
            {
                
            });
        }
    }
}
