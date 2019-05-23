using SF.Sys.Entities;
using System.Threading.Tasks;

namespace SF.Biz.Trades
{

    public class TradeDeliveryCreateArgument
    {
        public string Name { get; set; }
        public long SenderId { get; set; }
        public long ReceiverId { get; set; }

        public long DestAddressId { get; set; }

        public TrackIdent BizParent { get; set; }
        public TrackIdent BizRoot { get; set; }

        public TradeDeliveryItemCreateArgument[] Items { get; set; }
    }
    public class TradeDeliveryItemCreateArgument
    {
        public string Name { get; set; }
        public string Image { get; set; }
        public string PayloadEntityIdent { get; set; }
        public int Quantity { get; set; }
    }
  
    public enum TradeDeliveryState
    {
        WaitDeliverying,
        WaitReceived,
        Success,
        Failed
    }
    public class TradeDeliveryResult
    {
        public long DeliveryId { get; set; }
        public TradeDeliveryState State { get; set; }
        public string Error { get; set; }
    }
    public interface ITradeDeliveryProvider
    {
        bool DeliveryAddressRequired { get; }
        Task<TradeDeliveryResult> Create(TradeDeliveryCreateArgument Argument);
        Task<TradeDeliveryResult> QueryStatus(TrackIdent BizParent);
    }
    
}
