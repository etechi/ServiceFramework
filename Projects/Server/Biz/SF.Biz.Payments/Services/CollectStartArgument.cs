using SF.Sys.Clients;

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
		public string ClientType { get; set; }
		public string TrackEntityIdent { get; set; }
		public long CurUserId { get; set; }
        public string CallbackUrl { get; set; }
        public string OpAddress { get; set; }
        public ClientDeviceType OpDevice { get; set; }
	}

}
