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
  
    [NetworkService]
    [AnonymousAllowed]
    public interface IPaymentPlatformService
    {
        Task<PaymentPlatform[]> List(ClientDeviceType Type);
        TimeSpan? GetCollectRequestTimeout(long Id);

    }

}
