using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using SF.Sys.Entities;
using SF.Sys.Clients;
using SF.Sys.Annotations;
using SF.Sys.Auth;
using SF.Sys.Services.Management.Models;
using SF.Sys.NetworkService;

namespace SF.Biz.Payments
{
    /// <summary>
    /// 支付平台服务
    /// </summary>
    [NetworkService]
    [AnonymousAllowed]
    public interface IPaymentPlatformService
    {
        /// <summary>
        /// 列表
        /// </summary>
        /// <returns>支付平台</returns>
        Task<PaymentPlatform[]> List();

        /// <summary>
        /// 获取收款请求超时
        /// </summary>
        /// <param name="Id">支付平台ID</param>
        /// <returns></returns>
        TimeSpan? GetCollectRequestTimeout(long Id);

    }

}
