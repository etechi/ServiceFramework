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
using SF.Biz.ShoppingCarts;
using SF.Sys.Clients;

namespace SF.Biz.UnitTest
{
    [TestClass]

    public class 购物车测试 : TestBase
	{

        public async Task Sync(IServiceProvider sp,long user, ItemStatus[] status)
        {
            await sp.WithScopedServices(async ((IAccessToken accToken, IShoppingCartService scs) svcs) =>
            {
                return await svcs.accToken.UseUser(user, async () =>
                 {
                     await svcs.scs.Sync(new SyncArguments
                     {
                         Items = status
                     });
                     return 0;
                 });
            });
        }
        
        public async Task Validate(IServiceProvider sp,long user, ItemStatus[] status)
        {
            await sp.WithScopedServices(async ((IAccessToken accToken, IShoppingCartService scs) svcs) =>
            {
                return await svcs.accToken.UseUser(user, async () =>
                {
                    var citems = await svcs.scs.List(new QueryArguments { });
                    Assert.AreEqual(citems.Length, status.Length);
                    foreach (var s in status)
                    {
                        Assert.IsNotNull(citems.Single(i => i.ItemId == s.ItemId && i.Quantity == s.Quantity && i.Selected == s.Selected));
                    }
                    return 0;
                });
            });

        }
        [TestMethod]
        public async Task 同步测试()
        {
            await (from sp in NewServiceScope().InitServices()
                   from user in sp.UserCreate()
                   from pro in sp.ProductEnsure(user.User.Id)
                   select (sp, user, pro))
               .Use(async (ctx) =>
               {
                   (IServiceProvider sp, Common.UnitTest.UserInfo user, ProductInternal product) = ctx;
                   var items = await sp.ProductItemsEnsure(user.User.Id, 7);

                   var status = new[] {
                       new ItemStatus { ItemId = items[0].Id, Quantity = 1, Selected = true },
                       new ItemStatus { ItemId = items[1].Id, Quantity = 2, Selected = true },
                       new ItemStatus { ItemId = items[2].Id, Quantity = 4, Selected = false },
                       new ItemStatus { ItemId = items[3].Id, Quantity = 8, Selected = true }
                   };
                   //Init  
                   await Sync(sp, user.User.Id, status);
                   await Validate(sp, user.User.Id, status);

                   //Update
                   status[1].Quantity = 20;
                   await Sync(sp, user.User.Id, status);
                   await Validate(sp, user.User.Id, status);

                   //Add
                   status = status.Concat(
                       new[] {
                           new ItemStatus { ItemId = items[4].Id, Quantity = 16, Selected = false },
                           new ItemStatus { ItemId = items[5].Id, Quantity = 32, Selected = true }
                       }
                   );
                   await Sync(sp, user.User.Id, status);
                   await Validate(sp, user.User.Id, status);

                   //Remove
                   status = status.Copy(0, status.Length - 1);
                   await Sync(sp, user.User.Id, status);
                   await Validate(sp, user.User.Id, status);

                   //UpdateAddRemove
                   status = status.Copy(0, status.Length - 1).Concat(
                       new[] {
                           new ItemStatus { ItemId = items[6].Id, Quantity = 64, Selected = true }
                       });
                   status[0].Quantity = 100;
                   await Sync(sp, user.User.Id, status);
                   await Validate(sp, user.User.Id, status);


                   //Clear
                   status = new ItemStatus[0];
                   await Sync(sp, user.User.Id, status);
                   await Validate(sp, user.User.Id, status);
               });
        }


    }


}
