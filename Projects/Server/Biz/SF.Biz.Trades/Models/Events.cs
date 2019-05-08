namespace SF.Biz.Accounting
{
    /// <summary>
    /// 充值完成
    /// </summary>
    public class AccountDepositCompleted
    {
        public long Ident { get; }//AccountDepositRecord-xxx
        public string Desc { get; }
        public decimal Amount { get; }
        public long UserId { get; }
        public string AccountTitle { get; }
        public AccountDepositCompleted(long id, long UserId, string Desc, decimal Amount, string AccountTitle)
        {
            this.Ident = id;
            this.UserId = UserId;
            this.Desc = Desc;
            this.Amount = Amount;
            this.AccountTitle = AccountTitle;
        }
    }
    /// <summary>
    /// 退款完成
    /// </summary>
    public class AccountRefundCompleted
    {
        public long Ident { get; }//AccountDepositRecord-xxx
        public string Desc { get; }
        public decimal Amount { get; }
        public long UserId { get; }
        public bool Success { get; }
        public AccountRefundCompleted(long id, long UserId, string Desc, decimal Amount, bool Success)
        {
            this.Ident = id;
            this.UserId = UserId;
            this.Desc = Desc;
            this.Amount = Amount;
            this.Success = Success;
        }
    }
}
