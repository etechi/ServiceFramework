using System;

namespace SF.Biz.Payments
{
    /// <summary>
    /// 收款会话
    /// </summary>
    public class CollectSession
    {
        public long Id { get; set; }

        public DateTime CreateTime { get; set; }
        public string ExtraData { get; set; }
        public CollectStartArgument Request { get; set; }
        public CollectResponse Response { get; set; }
    }

}
