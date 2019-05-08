﻿using System;
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
using SF.Sys.Services.Management;
using SF.Sys.Services;

namespace SF.Biz.Payments
{
  
    public class PaymentPlatformService: IPaymentPlatformService
    {
        public IServiceInstanceManager ServiceInstanceManager { get; }
        public TypedInstanceResolver<ICollectProvider> CollectProviderResolver { get; }
        public PaymentPlatformService(IServiceInstanceManager ServiceInstanceManager, TypedInstanceResolver<ICollectProvider> CollectProviderResolver)
        {
            this.ServiceInstanceManager = ServiceInstanceManager;
            this.CollectProviderResolver = CollectProviderResolver;
        }
        public async Task<PaymentPlatform[]> List(ClientDeviceType Type)
        {
            var svcs=await ServiceInstanceManager.QueryAsync(new ServiceInstanceQueryArgument
            {
                ServiceType = typeof(ICollectProvider).FullName,
                LogicState=EntityLogicState.Enabled

            });
            return svcs.Items.OrderBy(i => i.ItemOrder).Select(i => new PaymentPlatform
            {
                Id=i.Id,
                Icon=i.Icon,
                Image=i.Image,
                Name=i.Name,
                Title=i.Title,
                CreatedTime=i.CreatedTime,
                Description=i.Description,
                Remarks=i.Remarks,
                SubTitle=i.SubTitle,
                UpdatedTime=i.UpdatedTime,
                LogicState=i.LogicState

            }).ToArray();
        }

        public TimeSpan? GetCollectRequestTimeout(long Id)
        {
            return CollectProviderResolver(Id).CollectRequestTimeout;
        }
    }

}
