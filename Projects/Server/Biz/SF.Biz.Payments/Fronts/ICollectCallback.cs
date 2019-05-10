using System.Net.Http;
using System.Threading.Tasks;
using SF.Sys.Auth;
using SF.Sys.NetworkService;

namespace SF.Biz.Payments
{
    /// <summary>
    /// 第三方回调接口
    /// </summary>
    [NetworkService]
    [AnonymousAllowed]
    public interface ICollectCallback
    {
        /// <summary>
        /// 第三方易步通知
        /// </summary>
        /// <param name="Id">平台ID</param>
        /// <returns>通知应答</returns>
        Task<HttpResponseMessage> Notify(long Id);
        /// <summary>
        /// 第三方页面返回
        /// </summary>
        /// <param name="Id">平台ID</param>
        /// <returns>页面返回应答</returns>
        Task<HttpResponseMessage> Return(long Id);
    }
}
