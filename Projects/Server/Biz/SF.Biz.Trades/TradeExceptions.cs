using SF.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Biz.Trades
{
    public class TradeSettlementException : PublicInvalidOperationException
    {
        public TradeSettlementException(string message) : base(message) { }
        public TradeSettlementException(string message, System.Exception innerException) : base(message, innerException) { }
    }
    public class TradeException : PublicException
	{
		public TradeException(string message) : base(message) { }
		public TradeException(string message, System.Exception innerException) : base(message, innerException) { }

    }
    public class TradeCreateException : PublicInvalidOperationException
    {
        public TradeCreateException(string message) : base(message) { }
        public TradeCreateException(string message, System.Exception innerException) : base(message, innerException) { }
    }
}
