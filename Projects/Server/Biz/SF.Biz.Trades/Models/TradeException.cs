using SF.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Biz.Trades
{
	
	public class TradeException : PublicException
	{
		public TradeException(string message) : base(message) { }
		public TradeException(string message, System.Exception innerException) : base(message, innerException) { }

	}
}
