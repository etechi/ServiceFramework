using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using SF.Sys.Clients;
using SF.Sys.Entities;
using SF.Sys.Annotations;
using SF.Sys.Auth;
using SF.Sys.Services.Management.Models;
using SF.Sys.NetworkService;

namespace SF.Biz.Payments
{
    public class CollectComplete
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long UserId { get; set; }
        public decimal Amount { get; set; }
        public decimal AmountCompleted { get; set; }

    }

    public class StartRequestInfo
	{
		public Uri Uri { get; set; }
		public string ClientAddress { get; set; }
        public ClientDeviceType DeviceType { get; set; }

		//public static RequestInfo CreateFromRequest(System.Net.Http.HttpRequestMessage Request)
		//{
		//	var ctx = System.Web.HttpContext.Current;
		//	return new RequestInfo
		//	{
		//		Uri = Request.RequestUri,
		//		ClientAddress = ctx?.Request?.UserHostAddress ?? "0.0.0.0",
  //              DevivceType=Request.det
  //          };
		//}
	}
	public class CollectRequest : CollectStartArgument
    {
        public string CallbackName { get; set; }
		public string CallbackContext { get; set; }
	}
	public class CollectStatus 
	{
		public string Title { get; set; }
		public decimal Amount { get; set; }
	}
	
	public class QrCodePaymentStatus
	{
		public string Title { get; set; }
		public decimal Amount { get; set; }
		public long Ident { get; set; }
		public DateTime CreateTime { get; set; }
		public long PaymentPlatform { get; set; }
		public string QrCodeUri { get; set; }
		public DateTime? CompletedTime { get; set; }
		public string HttpRedirect { get; set; }
	}
	public interface ICollectService
    {
        /// <summary>
        /// 创建收款会话
        /// </summary>
        /// <param name="Argument">收款请求参数</param>
        /// <returns></returns>
		Task<long> Create(CollectRequest Argument);

        /// <summary>
        /// 开始收款操作
        /// </summary>
        /// <param name="Id">收款ID</param>
        /// <param name="RequestInfo">收款开始请求信息</param>
        /// <returns></returns>
		Task<IDictionary<string,string>> Start(long Id, StartRequestInfo RequestInfo);

        /// <summary>
        /// 查询第三方收款状态
        /// </summary>
        /// <param name="Id">收款ID</param>
        /// <returns></returns>
		Task<CollectSession> TryCompleteByQuery(long Id);
        
        /// <summary>
        /// 最后一次查询第三方收款状态
        /// </summary>
        /// <param name="Id">收款ID</param>
        /// <returns></returns>
        Task<CollectSession> LastTryCompleteByQuery(long Id);

        /// <summary>
        /// 获取收款结果
        /// </summary>
        /// <param name="Id">收款ID</param>
        /// <returns></returns>
        Task<CollectSession> GetResult(long Id);

        /// <summary>
        /// 获取收款提供者参数
        /// </summary>
        /// <param name="Id">收款ID</param>
        /// <returns></returns>
		Task<string> GetStartProviderExtraData(long Id);

        /// <summary>
        /// 获取收款二维码
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
		Task<QrCodePaymentStatus> GetQrCodePaymentStatus(long Id);
	}

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
