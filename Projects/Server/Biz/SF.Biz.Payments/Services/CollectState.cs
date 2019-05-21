namespace SF.Biz.Payments
{
    public enum CollectState
    {
        /// <summary>
        /// 新建
        /// </summary>
        Init,
        /// <summary>
        /// 收款中
        /// </summary>
        Collecting,
        /// <summary>
        /// 成功
        /// </summary>
        Success,
        /// <summary>
        /// 失败
        /// </summary>
        Failed,
        /// <summary>
        /// 取消
        /// </summary>
        Canceled
    }
}
