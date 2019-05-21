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
using SF.UT;
using System.Linq;
using SF.Sys.UnitTest;
using SF.Common.UnitTest;
using SF.Sys;
using SF.Biz.Products;
using SF.Sys.Entities;
using System.Collections.Generic;
using SF.Biz.Trades;
using SF.Sys.Auth;

namespace SF.Biz.UnitTest
{
    [TestClass]

    public class 交易测试 : TestBase
    {
        async Task use_test_scope(Func<IServiceProvider,User,User,Task> callback)
        {
            await(from sp in NewServiceScope().InitServices().InitData()
                  from buyer in sp.UserCreate()
                  from seller in sp.UserCreate()
                  select (sp, buyer, seller))
                  .Use((ctx) => callback(ctx.sp, ctx.buyer.User, ctx.seller.User));
        }

        [TestMethod]
        public async Task 简单交易_余额()
        {
            await use_test_scope(async (sp, buyer,seller)=>
            {
                var items = await sp.ProductItemsEnsure(seller.Id, 1);
                var trade = await sp.ExecTrade(
                    seller.Id,
                    buyer.Id,
                    1000,
                    new[]{
                        ("Item-"+items[0].Id,10)
                    },
                    1000
                    );
                Assert.AreEqual(TradeState.Closed, trade.State);
            });
        }
        [TestMethod]
        public async Task 简单交易_直接支付()
        {
            await use_test_scope(async (sp, buyer, seller) =>
            {
                var items = await sp.ProductItemsEnsure(seller.Id, 1);
                var trade = await sp.ExecTrade(
                    seller.Id,
                    buyer.Id,
                    1000,
                    new[]{("Item-"+items[0].Id,10)},
                    1000,
                    useBalance:false                             
                    );
                Assert.AreEqual(TradeState.Closed, trade.State);
            });
        }
        [TestMethod]
        public async Task 简单交易_余额加支付()
        {
            await use_test_scope(async (sp, buyer, seller) =>
            {
                var items = await sp.ProductItemsEnsure(seller.Id, 1);
                var trade = await sp.ExecTrade(
                    seller.Id,
                    buyer.Id,
                    1000,
                    new[] { ("Item-" + items[0].Id, 10) },
                    500,
                    useBalance: true
                    );
                Assert.AreEqual(TradeState.Closed, trade.State);
            });
        }
    }
}
