#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0

using System.Threading.Tasks;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SF.Sys.UnitTest;
using SF.Sys.Hosting;
using SF.Auth.IdentityServices;
using SF.Sys.Services;
using SF.Auth.IdentityServices.Externals;
using SF.Sys.Data;
using SF.Sys;
using SF.Biz.Products;
using System.Linq;
using SF.Sys.Entities;
using SF.Sys.Linq;
using System.Collections.Generic;
using SF.Biz.Accounting;
using SF.Biz.Payments;
using SF.Sys.Clients;
using SF.Sys.Collections.Generic;
using SF.Sys.NetworkService;

namespace SF.Biz.UnitTest
{
	
	public static class AccountingTestExtensions
	{
        public static async Task<decimal> Deposit(this IServiceProvider sp,long user, decimal amount)
        {
            var depositResult = await sp.WithScope(isp =>
              {
                  var access_token = isp.Resolve<IAccessToken>();
                  return access_token.UseUser(user, async () =>
                  {
                      var acc_service = isp.Resolve<IAccountingService>();
                      var balance = await acc_service.Balance();
                      var pps = isp.Resolve<IPaymentPlatformService>();
                      var platforms = await pps.List(Sys.Clients.ClientDeviceType.PCBrowser);
                      var platform = platforms.Where(p => p.Title == "支付测试").Single();

                      
                      var result = await acc_service.Deposit(new ClientDepositArguments
                      {
                          Amount = amount,
                          ClientType = "test",
                          PaymentPlatformId = platform.Id,
                          Redirect = "/asdasd/"
                      });
                      Assert.IsNotNull(result);
                      var recs = await acc_service.QueryDepositRecords(
                          new ClientDepositRecordQueryArguments
                          {
                              Id = result.Id
                          });

                      var rec = recs.Items.Single();
                      Assert.AreEqual(DepositState.Processing, rec.State);

                      return new { result, platform,org_balance=balance };
                  });
              });


            var pid = depositResult.result.PaymentStartResult.Get("id");
            var redirect = depositResult.result.PaymentStartResult.Get("redirect");
            Assert.IsNotNull(redirect);

            await sp.WithScope(async isp =>
            {
                var callback = isp.Resolve<ICollectCallback>();
                var ctx = isp.Resolve<ILocalInvokeContext>();
                ctx.Request.Uri = depositResult.result.PaymentStartResult.Get("unittest-notify");
                await callback.Notify(depositResult.platform.Id);
                return 0;
            });

            return await sp.WithScope(isp =>
            {
                var access_token = isp.Resolve<IAccessToken>();
                return access_token.UseUser(user, async () =>
                {
                    var acc_service = isp.Resolve<IAccountingService>();
                    var new_balance = await acc_service.Balance();
                    var recs = await acc_service.QueryDepositRecords(
                        new ClientDepositRecordQueryArguments
                        {
                            Id = depositResult.result.Id
                        });

                    var rec = recs.Items.Single();
                    Assert.AreEqual(DepositState.Completed, rec.State);
                    Assert.AreEqual(new_balance, depositResult.org_balance + amount);

                    return new_balance;
                });
            });
        }


    }


}
