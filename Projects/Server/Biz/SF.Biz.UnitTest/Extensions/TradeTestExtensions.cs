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
using SF.Biz.Trades;
using SF.Biz.ShoppingCarts;
using SF.Common.UnitTest;
using SF.Biz.Payments;
using SF.Biz.Accounting;

namespace SF.Biz.UnitTest
{
	
	public static class TradeTestExtensions
	{
        public static async Task<Trade> ExecTrade(
             this IServiceProvider sp,
             long seller,
             long buyer,             
             decimal amount,
             (string item,int quantity)[] items,
             decimal balance=0,
             bool useBalance=true,
             string discountCode=null,
             int discountCount=0
             )
        {

            if(balance>0)
                await sp.Deposit(buyer, balance);

            var tradeId=await sp.SyncShoppingCart(buyer, items ,true);


            var paymentResult=await sp.WithUserScope(buyer, async isp =>
            {
                var bts = isp.Resolve<IBuyerTradeService>();
                var trade = await bts.Get(tradeId, true);

                Assert.AreEqual(amount, trade.SettlementAmount);

                var pps = isp.Resolve<IPaymentPlatformService>();
                var platforms = await pps.List();
                var args = new PaymentArgument
                {
                    PaymentPlatformId = platforms.SingleOrDefault(p => p.Title == "支付测试").Id,
                    TradeId = trade.Id,
                    DiscountCode = discountCode,
                    DiscountCount = discountCount,
                    UseBalance = useBalance,
                    HttpRedirect = "http://localhost/"
                };
                var re = await bts.Payment(args);
                return (args.PaymentPlatformId,result:re);
            });

            decimal paymentAmount = 0;
            if (paymentResult.result.PaymentArguments != null)
            {                
                var query=await sp.TestPaymentNotify(paymentResult.PaymentPlatformId.Value, paymentResult.result.PaymentArguments);
                paymentAmount = query["amount"].ToDecimal();
            }
            Assert.AreEqual(amount - Math.Min(useBalance ? balance : 0, amount), paymentAmount);

            Assert.AreEqual(amount, await sp.GetAccountTitleValue("trade-prepay", seller));
            Assert.AreEqual(
                useBalance ? balance - Math.Min(amount, balance) : balance, 
                await sp.GetAccountTitleValue("balance", buyer)
                );

            Assert.AreEqual(0,await sp.GetAccountTitleValue("trade-collect", buyer));


            await sp.WithUserScope(seller, async isp =>
             {
                 var sts = sp.Resolve<ISellerTradeService>();
                 await sts.Delivery(tradeId);
                 return 0;
             });

            var retrade=await sp.WithUserScope(buyer, async isp =>
            {
                var sts = sp.Resolve<IBuyerTradeService>();
                await sts.Confirm(tradeId);
                return await sts.Get(tradeId,true);
            });

            Assert.AreEqual(amount,await sp.GetAccountTitleValue("balance", seller));

            return retrade;
        }
    }


}
