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

    public class 账户测试 : TestBase
    {
        [TestMethod]
        public async Task 充值()
        {
            await (from sp in NewServiceScope().InitServices().InitData()
                   from user in sp.UserCreate()
                   select (sp, user))
                     .Use(async (ctx) =>
                     {
                         var re1 = await ctx.sp.Deposit(ctx.user.User.Id, 100);
                         Assert.AreEqual(100, re1);
                         var re2 = await ctx.sp.Deposit(ctx.user.User.Id, 40);
                         Assert.AreEqual(140, re2);
                     });
        }
    }
}
