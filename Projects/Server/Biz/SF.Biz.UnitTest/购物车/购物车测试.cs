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

namespace SF.Biz.UnitTest
{
    [TestClass]

    public class 购物车测试 : TestBase
	{

        public async Task Sync(int user, ItemStatus[] status)
        {
            var scs = ShoppingCartService(DIScopeDefault);
            await scs.Sync(user, null, status);
        }
        public async Task<IItem[]> Items()
        {
            var items = (await ItemService(DIScopeDefault).ListItems(
              Setting(DIScopeDefault).MainCategoryId,
              true,
              null,
              new ServiceProtocol.ObjectManager.Paging { Count = 10 }
              )).Items.ToArray();
            return items;
        }
        public async Task Validate(int user, ItemStatus[] status)
        {
            var scs = ShoppingCartService(DIScopeDefault);
            var citems = await scs.List(user, null, false, false);
            Assert.Equal(citems.Length, status.Length);
            foreach (var s in status)
            {
                Assert.NotNull(citems.Single(i => i.ItemId == s.ItemId && i.Quantity == s.Quantity && i.Selected == s.Selected));
            }

        }
        [TestMethod]
        public async Task 同步测试()
        {
            var scs = DIScopeDefault.Resolve<CrowdMall.Bizness.ShoppingCarts.IShoppingCartService>();
            scs.DisableQuantityLimit = true;

            var user = await UserEnsure();
            var items = await Items();
            var status = new[] {
                new ItemStatus{ItemId=items[0].ItemId,Quantity=1,Selected=true },
                new ItemStatus{ItemId=items[1].ItemId,Quantity=2,Selected=true },
                new ItemStatus{ItemId=items[2].ItemId,Quantity=4,Selected=false },
                new ItemStatus{ItemId=items[3].ItemId,Quantity=8,Selected=true }
                };
            //Init
            await Sync(user.Id, status);
            await Validate(user.Id, status);

            //Update
            status[1].Quantity = 20;
            await Sync(user.Id, status);
            await Validate(user.Id, status);

            //Add
            status = status.Concat(
                new[] {
                new ItemStatus { ItemId = items[4].ItemId, Quantity = 16, Selected = false },
                new ItemStatus { ItemId = items[5].ItemId, Quantity = 32, Selected = true }
                }
            );
            await Sync(user.Id, status);
            await Validate(user.Id, status);

            //Remove
            status = status.Copy(0, status.Length - 1);
            await Sync(user.Id, status);
            await Validate(user.Id, status);

            //UpdateAddRemove
            status = status.Copy(0, status.Length - 1).Concat(
                new[] {
                new ItemStatus { ItemId = items[6].ItemId, Quantity = 64, Selected = true }
                });
            status[0].Quantity = 100;
            await Sync(user.Id, status);
            await Validate(user.Id, status);


            //Clear
            status = new ItemStatus[0];
            await Sync(user.Id, status);
            await Validate(user.Id, status);
        }


    }


}
