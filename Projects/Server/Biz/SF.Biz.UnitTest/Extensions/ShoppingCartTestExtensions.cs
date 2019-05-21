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

namespace SF.Biz.UnitTest
{
	
	public static class ShoppingCartTestExtensions
	{
        public static Task<long> SyncShoppingCart(
             this IServiceProvider sp,
             long buyer,
             (string item,int quantity)[] items,
             bool CreateTrade
             )
        {
            return sp.WithUserScope(buyer,async isp =>
            {
                var scs = isp.Resolve<IShoppingCartService>();
                await scs.Sync(new SyncArguments
                {
                    Items = items.Select(i => new ItemStatus
                    {
                        ItemId = i.item,
                        Quantity = i.quantity,
                        Selected = true
                    }).ToArray(),
                    
                });
                var exists= await scs.List(new QueryArguments { });
                Assert.AreEqual(
                    items.Select(i => $"{i.item}:{i.quantity}").Join(","),
                    exists.Select(i => $"{i.ItemId}:{i.Quantity}").Join(",")
                    );
                if (CreateTrade)
                    return await scs.CreateTrade(null);
                return 0L;
            });
        }
    }


}
