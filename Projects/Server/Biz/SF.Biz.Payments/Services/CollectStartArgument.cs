using SF.Sys.Clients;
using SF.Sys.Entities;

namespace SF.Biz.Payments
{
    /// <summary>
    /// 收款启动参数
    /// </summary>
    public class CollectStartArgument
	{
		public string Title { get; set; }
		public string Desc { get; set; }
		public decimal Amount { get; set; }
		public long PaymentPlatformId { get; set; }
		public string HttpRedirect { get; set; }

        public TrackIdent BizRoot { get; set; }
        public TrackIdent BizParent { get; set; }
        public ClientInfo ClientInfo { get; set; }
	}

}
