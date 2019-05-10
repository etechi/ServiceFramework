using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using SF.Sys.Clients;
using SF.Sys.Entities;
using SF.Sys.Annotations;
using SF.Sys.Services.Management.Models;

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
        public long RemindId { get; set; }
        public DateTime? StartTime { get; set; }
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
        /// 获取收款结果
        /// </summary>
        /// <param name="Id">收款ID</param>
        /// <param name="Query">是否查询第三方平台的交易状态</param>
        /// <param name="Remind">交易完成时是否激活提醒</param>
        /// <returns></returns>
        Task<CollectSession> GetResult(long Id,bool Query=false,bool Remind=false);

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
}
