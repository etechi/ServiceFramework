using SF.Common.Addresses.Data;
using SF.Common.Addresses.Management;
using SF.Sys.Collections.Generic;
using SF.Sys.Data;
using SF.Sys.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using SF.Sys.Entities;

namespace SF.Common.Addresses.Init
{
    public static class Setup
    {
        public static async Task EnsureLocations(this IServiceProvider sp,bool useTransaction=true,bool testMode=false)
        {
            await sp.Resolve<IDataScope>().Use("初始化地区数据", async context =>
            {
                var items = LocationData.Locations;
                var dlm = sp.Resolve<ILocationManager>();
                foreach (var it in items)
                {
                    if(testMode)
                    {
                        if ((it.L1Code != 37 || it.L2Code != 33 || it.L3Code != 1 ) &&
                            it.Id!=37000000 &&
                            it.Id!=37330000 &&
                            it.Id != 37330100
                            )
                            continue;
                    }
                    await dlm.EnsureEntity(
                        await dlm.QuerySingleEntityIdent(new LocationQueryArguments
                        {
                            Id = it.Id
                        }),
                        ()=>new Management.LocationInternal { Id= it.Id},
                        o =>
                        {
                            o.Id = it.Id;
                            o.ParentId = it.ParentId;
                            o.Order = it.Order;
                            o.Level = it.Level;
                            o.L1Code = it.L1Code;
                            o.L2Code = it.L2Code;
                            o.L3Code = it.L3Code;
                            o.L4Code = it.L4Code;
                            o.Name = it.Name;
                            o.EnName = it.EnName;
                            o.Code = it.Code;
                            o.PhonePrefix = it.PhonePrefix;
                            o.TimeZone = it.TimeZone;
                            o.LogicState = it.LogicState;
                        }
                        );
                }
                await context.SaveChangesAsync();
            }, TransactionLevel: useTransaction?System.Data.IsolationLevel.ReadCommitted: System.Data.IsolationLevel.Unspecified);
        }
    }
}
