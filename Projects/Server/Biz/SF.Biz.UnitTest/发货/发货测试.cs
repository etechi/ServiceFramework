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

namespace SF.Biz.UnitTest
{
    [TestClass]

    public class 发货测试 : TestBase
    {


        [TestMethod]
        public async Task 创建地址()
        {
            await (from sp in NewServiceScope().InitServices().InitData()
                   from user in sp.UserCreate()
                   select (sp, user))
                     .Use(async (ctx) =>
                     {
                         var addr = await ctx.sp.CreateUserAddress(ctx.user.User.Id);
                     });
        }
        //[TestMethodAttribute]
        //public async Task 创建发货()
        //{
        //    await OpenRound(async (r) =>
        //    {
        //        var addr = await base.CreateAddress(r.WinSummary.User.Id, null);
        //        await base.CreateDelivery(r.ProductId, r.Round, addr.Id, null);
        //        return 0;
        //    }, null);
        //}
        //[TestMethodAttribute]
        //public async Task 发货()
        //{
        //    await OpenRound(async (r) =>
        //    {
        //        var addr = await base.CreateAddress(r.WinSummary.User.Id, null);
        //        var did = await base.CreateDelivery(r.ProductId, r.Round, addr.Id, null);
        //        await base.DeliverySended(did, int.Parse(DIScopeDefault.Resolve<ServiceProtocol.Auth.AuthSetting>().SysAdminId), null);
        //        return 0;
        //    },
        //    null);
        //}
        //[TestMethodAttribute]
        //public async Task 发货所有()
        //{
        //    var recs = await DeliveryService(null).Query(
        //        new ServiceProtocol.Biz.Delivery.DeliveryQueryArguments<int>
        //        {
        //            State = ServiceProtocol.Biz.Delivery.DeliveryState.DeliveryWaiting
        //        },
        //        new ServiceProtocol.ObjectManager.Paging { Count = 100 }
        //        );
        //    foreach (var rec in recs.Items)
        //        await base.DeliverySended(
        //            rec.Id,
        //            int.Parse(DIScopeDefault.Resolve<ServiceProtocol.Auth.AuthSetting>().SysAdminId),
        //            null
        //            );
        //}
        //[TestMethodAttribute]
        //public async Task 接收()
        //{
        //    await OpenRound(async (r) =>
        //    {
        //        var addr = await base.CreateAddress(r.WinSummary.User.Id, null);
        //        var did = await base.CreateDelivery(r.ProductId, r.Round, addr.Id, null);
        //        await base.DeliverySended(did, int.Parse(DIScopeDefault.Resolve<ServiceProtocol.Auth.AuthSetting>().SysAdminId), null);
        //        await base.DeliveryReceived(did, r.WinSummary.User.Id, null);
        //        return 0;
        //    }, null);
        //}
    }
}
