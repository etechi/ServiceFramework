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
using SF.Biz.Delivery;

namespace SF.Biz.UnitTest
{
	
	public static class DeliveryTestExtensions
	{

        public static async Task<UserAddress> CreateUserAddress(this IServiceProvider sp,long UserId)
        {
            var dls = sp.Resolve<IDeliveryLocationService>();

            var Name =Strings.LowerChars.Random(10);
            var Phone = "131"+ Strings.Numbers.Random(8);
            var Address = Strings.NumberAndLowerChars.Random(20);
            var r = new Random();
            var ps = await dls.List(null);
            var p = ps[r.Next(ps.Length)];
            var cs = await dls.List(p.Id);
            var c = cs[r.Next(cs.Length)];
            var ds = await dls.List(c.Id);
            var d = ds[r.Next(ds.Length)];

            var addr = new UserAddressEditable
            {
                Address = Address,
                CityId = c.Id,
                ContactName = Name,
                ContactPhoneNumber = Phone,
                DistrictId = d.Id,
                IsDefaultAddress = false,
                ProvinceId = p.Id,
            };

            var uas = sp.Resolve<IUserAddressService>();

            var id = await uas.UpdateAddress(addr);
            var re = await uas.LoadForEditAsync(id);
            Assert.AreEqual(Address, re.Address);
            Assert.AreEqual(Name, re.ContactName);
            Assert.AreEqual(Phone, re.ContactPhoneNumber);
            Assert.AreEqual(p.Id, re.ProvinceId);
            Assert.AreEqual(c.Id, re.CityId);
            Assert.AreEqual(d.Id, re.DistrictId);
            Assert.AreEqual(true, re.IsDefaultAddress);

            var re1 = await uas.GetUserAddress(id);
            Assert.AreEqual(Address, re1.Address);
            Assert.AreEqual(Name, re1.ContactName);
            Assert.AreEqual(Phone, re1.ContactPhoneNumber);
            Assert.AreEqual(d.Id, re1.LocationId);
            Assert.IsTrue(re1.LocationName.Contains(d.Name));
            Assert.IsTrue(re1.LocationName.Contains(c.Name));
            Assert.IsTrue(re1.LocationName.Contains(p.Name));
            Assert.AreEqual(true, re1.IsDefaultAddress);
            return re1;

        }

    }


}
