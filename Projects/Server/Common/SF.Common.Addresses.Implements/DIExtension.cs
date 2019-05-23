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

using SF.Common.Addresses;
using SF.Common.Addresses.Init;
using SF.Common.Addresses.Management;
using SF.Sys.BackEndConsole;
using SF.Sys.Collections.Generic;
using SF.Sys.Data;

namespace SF.Sys.Services
{
    public static class AddressDIExtension
		
	{
		public static IServiceCollection AddAddressServices(this IServiceCollection sc, string TablePrefix = null)
		{
            sc.EntityServices(
				"Addresses",
				"地址管理",
				d =>d.Add<IUserAddressManager, UserAddressManager>("UserAddress", "用户地址", typeof(UserAddressInternal))
                    .Add<IAddressSnapshotManager, AddressSnapshotManager>("DeliveryAddressSnapshot", "地址快照", typeof(AddressSnapshot))
                    .Add<ILocationManager, LocationManager>("DeliveryLocation", "地区", typeof(LocationInternal))
                );

			sc.AddManagedScoped<ILocationService, DeliveryLocationService>();
            sc.AddManagedScoped<IUserAddressService, UserAddressService>();
            sc.AddDataModules<
                SF.Common.Addresses.DataModels.DataAddress,
                SF.Common.Addresses.DataModels.DataAddressSnapshot,
                SF.Common.Addresses.DataModels.DataLocation
                >(TablePrefix ?? "Biz");

			sc.InitServices("Delivery", async (sp, sim, parent) =>
			 {
				
                 await sim.DefaultService<IUserAddressManager, UserAddressManager>(null)
                    .WithConsolePages("地址管理/用户地址管理")
                    .Ensure(sp, parent);

                 await sim.DefaultService<IAddressSnapshotManager, AddressSnapshotManager>(null)
                    .WithConsolePages("地址管理/地址快照管理")
                    .Ensure(sp, parent);

                 await sim.DefaultService<ILocationManager, LocationManager>(null)
                    .WithConsolePages("地址管理/地区管理")
                    .Ensure(sp, parent);

                 await sim.DefaultService<ILocationService, DeliveryLocationService>(null)
                    .Ensure(sp, parent);

                 await sim.DefaultService<IUserAddressService, UserAddressService>(null)
                    .Ensure(sp, parent);
            });

            sc.AddInitializer("data", "delivery", async (sp,args) =>
            {
                var useTransaction = args.Get("disableTransaction") != "true";
                var testMode = args.Get("unittest") == "true";
                if (testMode)
                    useTransaction = false;

                await sp.EnsureLocations(useTransaction,testMode);
            });

			return sc;
		}
	}
}
