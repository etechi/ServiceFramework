using System;

namespace SF.Biz.Payments
{
    /// <summary>
    /// 收款应答
    /// </summary>
    public class CollectResponse
    {
        public long Ident { get; set; }
        public DateTime? CompletedTime { get; set; }
        public string ExtUserId { get; set; }
        public string ExtUserName { get; set; }
        public string ExtUserAccount { get; set; }
        public string ExtIdent { get; set; }
        public decimal AmountCollected { get; set; }
        public string Error { get; set; }
    }

}
