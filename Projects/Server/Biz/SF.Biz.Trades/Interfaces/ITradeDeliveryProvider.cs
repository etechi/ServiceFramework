using SF.Sys.Entities;
using System.Threading.Tasks;

namespace SF.Biz.Trades
{
    public class StartDeliveryItem
    {
        public string ProductId { get; set; }
        public int Quantity { get; set; }
    }
    public class StartDeliveryArgument
    {
        public TrackIdent BizRoot { get; set; }
        public TrackIdent BizParent { get; set; }
        public long? DeliveryAddressId { get; set; }
        public StartDeliveryItem[] Items { get; set; }
    }
    public enum TradeDeliveryState
    {
        Processing,
        Success,
        Failed
    }
    public class TradeDeliveryStartResult
    {
        public string Id { get; set; }
        public TradeDeliveryState State { get; set; }
    }
    public interface ITradeDeliveryProvider
    {
        bool DeliveryAddressRequired { get; }
        Task<TradeDeliveryStartResult> Start(StartDeliveryArgument Argument);
        Task<TradeDeliveryStartResult> QueryStatus(string BizParent);
    }
    
}
