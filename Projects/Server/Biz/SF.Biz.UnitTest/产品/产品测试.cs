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

    public class 产品测试 : TestBase
	{
        

        [TestMethod]
        public async Task 新建()
        {
            await (from sp in NewServiceScope().InitServices()
                   from user in sp.UserCreate()
                   select (sp, user))
                   .Use(async (ctx) =>
                   {
                       var (sp, env) = ctx;
                       var product = await sp.ProductEnsure(env.User.Id);
                       await sp.ProductEnable(product.Id);
                       var item = await sp.ProductItemEnsure(env.User.Id, product.Id);
                       var re = await sp.Resolve<IItemService>().GetItem(item.Id);
                       Assert.AreEqual(item.Id, re.ItemId);
                       Assert.AreEqual(product.Id, re.ProductId);
                   });
        }
        [TestMethod]
        public async Task 修改产品()
        {
            await (from sp in NewServiceScope().InitServices()
                   from user in sp.UserCreate()
                   from pro in sp.ProductEnsure(user.User.Id)
                    select (sp, user,pro))
                   .Use(async (ctx) =>
                   {
                       var (sp, env, productInternal) = ctx;
                       
                       await sp.ProductEnable(productInternal.Id);
                       var item = await sp.ProductItemEnsure(env.User.Id, productInternal.Id);
                       var re = await sp.Resolve<IItemService>().GetItem(item.Id);
                       Assert.AreEqual(item.Id, re.ItemId);
                       Assert.AreEqual(productInternal.Id, re.ProductId);

                       var old_item = await sp.Resolve<IItemService>().GetItem(item.Id);
                       var pm = sp.Resolve<IProductManager>();
                       Assert.AreEqual(productInternal.Title, old_item.Title);

                       var new_name = productInternal.Name+ " changed";
                       var new_title = old_item.Title + " changed";
                       var new_image = old_item.Image + " changed";
                       await pm.UpdateEntity(ObjectKey.From(productInternal.Id), e =>
                       {
                           e.Name = new_name;
                           e.Title = new_title;
                           e.Image = new_image;
                       });
                       var new_item = await sp.Resolve<IItemService>().GetItem(item.Id);
                       var new_product = await sp.Resolve<IProductManager>().GetAsync(ObjectKey.From(item.ProductId));
                       Assert.AreEqual(new_name, new_product.Name);
                       Assert.AreEqual(new_title, new_item.Title);
                       Assert.AreEqual(new_image, new_item.Image);

                   });
        }
        [TestMethod]
        public async Task 列表()
        {
            await (from sp in NewServiceScope().InitServices()
                   from user in sp.UserCreate()
                   select (sp, user))
                     .Use(async (ctx) =>
                     {
                         var (sp, env) = ctx;
                         //var seller = await UserEnsure();
                         //var setting = DIScopeDefault.Resolve<CrowdMall.CrowdMallSetting>();
                         //var iis = DIScopeDefault.Resolve<CrowdMall.Bizness.Products.IItemService>();
                         //var items1 = await iis.ListItems(setting.MainCategoryId, true, null, new ServiceProtocol.ObjectManager.Paging
                         //{
                         //    Offset = 0,
                         //    Count = 40,
                         //    SortMethod = "hot",
                         //    SortOrder = ServiceProtocol.ObjectManager.SortOrder.Desc
                         //});

                         //var items2 = await iis.ListItems(setting.MainCategoryId, true, null, new ServiceProtocol.ObjectManager.Paging
                         //{
                         //    Offset = 40,
                         //    Count = 40,
                         //    SortMethod = "hot",
                         //    SortOrder = ServiceProtocol.ObjectManager.SortOrder.Desc
                         //});

                         //Assert.True(
                         //    items1.Items.Select(i => i.ItemId).Any(i => items2.Items.All(i2 => i2.ItemId != i))
                         //    );
                     });
            
        }


    }


}
