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
using SF.Sys.Services;
using SF.Sys.Data;
using SF.UT;
using System.Linq;
using SF.Common.UnitTest;
using SF.Sys;
using SF.Biz.Products;
using SF.Sys.Auth;

namespace SF.Biz.UnitTest
{
    [TestClass]

    public class 交易测试 : TestBase
    {
        async Task use_test_scope(int itemCount, Func<IServiceProvider, User, User, ItemEditable[], Task> callback)
        {
            await (from sp in NewServiceScope().InitServices().InitData()
                   from buyer in sp.UserCreate()
                   from seller in sp.UserCreate()
                   from items in sp.ProductItemsEnsure(seller.User.Id, itemCount)
                   select (sp, buyer, seller, items))
                  .Use(async (ctx) =>
                  {
                      await ctx.sp.CreateUserAddress(ctx.buyer.User.Id);
                      await ctx.sp.CreateUserAddress(ctx.seller.User.Id);

                      await callback(ctx.sp, ctx.buyer.User, ctx.seller.User, ctx.items);
                  }
                );
        }

        [TestMethod]
        public async Task 简单交易_余额()
        {
            await use_test_scope(1,async (sp, buyer,seller,items)=>
            {
                var trade = await sp.ExecTrade(
                    seller.Id,
                    buyer.Id,
                    1000,
                    new[]{
                        ("Item-"+items[0].Id,10)
                    },
                    1000
                    );
            });
        }
        
        [TestMethod]
        public async Task 简单交易_充值()
        {
            await use_test_scope(1, async (sp, buyer, seller, items) =>
            {
                var trade = await sp.ExecTrade(
                    seller.Id,
                    buyer.Id,
                    1000,
                    new[]{("Item-"+items[0].Id,10)},
                    1000,
                    useBalance:false                             
                    );
            });
        }
        [TestMethod]
        public async Task 简单交易_余额加充值()
        {
            await use_test_scope(1, async (sp, buyer, seller, items) =>
            {
                var trade = await sp.ExecTrade(
                    seller.Id,
                    buyer.Id,
                    1000,
                    new[] { ("Item-" + items[0].Id, 10) },
                    500,
                    useBalance: true
                    );
            });
        }

        [TestMethod]
        public async Task 买家确认超时()
        {
            await use_test_scope(1, async (sp, buyer, seller, items) =>
            {
                var trade = await sp.ExecTrade(
                    seller.Id,
                    buyer.Id,
                    1000,
                    new[]{
                        ("Item-"+items[0].Id,10)
                    },
                    1000,
                    buyerConfirmTimeout:true
                    );
            });
        }
        [TestMethod]
        public async Task 买家余额_卖家取消发货()
        {
            await use_test_scope(1, async (sp, buyer, seller, items) =>
            {
                var trade = await sp.ExecTrade(
                    seller.Id,
                    buyer.Id,
                    1000,
                    new[]{
                        ("Item-"+items[0].Id,10)
                    },
                    1000,
                    useBalance:true,
                    sellerCompleteFailed:true
                    );
            });
        }
        [TestMethod]
        public async Task 买家充值_卖家取消发货()
        {
            await use_test_scope(1, async (sp, buyer, seller, items) =>
            {
                var trade = await sp.ExecTrade(
                    seller.Id,
                    buyer.Id,
                    1000,
                    new[]{
                        ("Item-"+items[0].Id,10)
                    },
                    0,
                    useBalance: false,
                    sellerCompleteFailed: true
                    );
            });
        }
        [TestMethod]
        public async Task 买家余额加充值_卖家取消发货()
        {
            await use_test_scope(1, async (sp, buyer, seller, items) =>
            {
                var trade = await sp.ExecTrade(
                    seller.Id,
                    buyer.Id,
                    1000,
                    new[]{
                        ("Item-"+items[0].Id,10)
                    },
                    600,
                    useBalance: true,
                    sellerCompleteFailed: true
                    );
            });
        }
    }
}
