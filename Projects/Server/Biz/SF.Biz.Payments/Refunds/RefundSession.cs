using System;

namespace SF.Biz.Payments
{
    public class RefundSession
    {
        public DateTime CreateTime { get; set; }
        public string ExtraData { get; set; }
        public CollectStartArgument Request { get; set; }
        public CollectResponse Response { get; set; }
    }


}
