using SF.Sys;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Biz.Trades.Services
{
    /// <summary>
    /// 交易有效性验证
    /// </summary>
    public class TradeValidator : ITradeValidator
    {
        void VerifyDiscount(
                string Name,
                decimal OrgValue,
                decimal SettlementValue,
                string DiscountDesc,
                string DiscountEntityId
                )
        {
            if (OrgValue == SettlementValue)
                return;
            if (OrgValue < SettlementValue)
                throw new PublicArgumentException($"结算{Name}不能比原始{Name}高");
            if (string.IsNullOrWhiteSpace(DiscountDesc))
                throw new PublicArgumentException($"结算{Name}比原始{Name}低时，必须提供折扣说明");
            if (string.IsNullOrWhiteSpace(DiscountEntityId))
                throw new PublicArgumentException($"结算{Name}比原始{Name}低时，必须提供折扣编号");
        }
        void VerifyRange(string Name, decimal Value, bool CanBeZero, decimal MaxValue)
        {
            if (CanBeZero)
            {
                if (Value < 0)
                    throw new PublicArgumentException($"{Name}不能小于0");
            }
            else if (Value <= 0)
                throw new PublicArgumentException($"{Name}不能小于等于0");

            if (Value > MaxValue)
                throw new PublicArgumentException($"{Name}不能超过{MaxValue}");
        }
        protected virtual void OnVerifyTradeItemArgument(
            TradeInternal trade,
            TradeItemInternal TradeItem
            )
        {
            Ensure.HasContent(TradeItem.Name, "必须提供订单项目描述");
            Ensure.HasContent(TradeItem.ProductId, "必须指定产品ID");


            VerifyRange("数量", TradeItem.Quantity, false, 1000000);
            VerifyRange("价格金额", TradeItem.Price, true, 10000000);
            var OrgAmount = TradeItem.PriceAfterDiscount * TradeItem.Quantity;
            VerifyRange("小计金额", OrgAmount, true, 10000000);
            VerifyRange("小计结算金额", TradeItem.SettlementAmount, true, 10000000);


            VerifyDiscount("单价金额", TradeItem.Price, TradeItem.PriceAfterDiscount, TradeItem.DiscountDesc, TradeItem.DiscountEntityIdent);

        }
        public Task Validate(TradeInternal trade)
        {
            Ensure.HasContent(trade.Name, "必须提供订单描述");

            Ensure.NotDefault(trade.SellerId, "必须提供卖家ID");
            Ensure.NotDefault(trade.BuyerId, "必须提供买家ID");
            Ensure.NotDefault(trade.SellerName, "必须提供卖家名称");
            Ensure.NotDefault(trade.BuyerName, "必须提供买家名称");

            Ensure.NotNull(trade.Items, "订单项");
            Ensure.Positive(trade.Items.Count(), "订单项数目");

            decimal amount = 0;
            var quantity = 0;
            foreach (var it in trade.Items)
            {
                OnVerifyTradeItemArgument(trade, it);
                amount += it.SettlementAmount;
                quantity += it.Quantity;
            }

            VerifyRange("总金额", amount, true, 10000000);
            VerifyRange("总结算金额", trade.SettlementAmount, true, 10000000);
            VerifyDiscount("总金额", amount, trade.SettlementAmount, trade.DiscountDesc, trade.DiscountEntityId);

            return Task.CompletedTask;
        }
    }

}
