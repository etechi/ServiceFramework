namespace SF.Biz.Payments
{
    public enum RefundState
    {
        /// <summary>
        /// 提交中
        /// </summary>
        Submitting,
        /// <summary>
        /// 处理中
        /// </summary>
        Processing,
        /// <summary>
        /// 成功
        /// </summary>
        Success,
        /// <summary>
        /// 失败
        /// </summary>
        Failed
    }
}
