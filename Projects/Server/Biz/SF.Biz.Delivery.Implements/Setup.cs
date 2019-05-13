using SF.Biz.Delivery.Data;
using SF.Biz.Delivery.Management;
using SF.Sys.Collections.Generic;
using SF.Sys.Data;
using SF.Sys.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using SF.Sys.Entities;

namespace SF.Biz.Delivery.Init
{
    public static class Setup
    {
        public static async Task EnsureTransport(this IServiceProvider sp, bool useTransaction = true)
        {

            await sp.Resolve<IDataScope>().Use("初始化快递公司数据", async context =>
            {
                var items = DeliveryTransportData.Items;
                var dtm = sp.Resolve<IDeliveryTransportManager>();
                foreach (var it in items)
                {
                    await dtm.EnsureEntity(
                        await dtm.QuerySingleEntityIdent(new DeliveryTransportQueryArguments
                        {
                            Code = it.Code
                        }),
                        o =>
                        {
                            o.Order = it.Order;
                            o.Name = it.Name;
                            o.Site = it.Site;
                            o.ContactName = it.ContactName;
                            o.ContactPhone = it.ContactPhone;
                            o.Code = it.Code;
                        });
                }
                await context.SaveChangesAsync();
            }, TransactionLevel: useTransaction ? System.Data.IsolationLevel.ReadCommitted : System.Data.IsolationLevel.Unspecified);
        }
        public static async Task EnsureLocations(this IServiceProvider sp,bool useTransaction=true,bool testMode=false)
        {
            await sp.Resolve<IDataScope>().Use("初始化地区数据", async context =>
            {
                var items = DeliveryLocationData.Locations;
                var dlm = sp.Resolve<IDeliveryLocationManager>();
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
                        await dlm.QuerySingleEntityIdent(new DeliveryLocationQueryArguments
                        {
                            Id = it.Id
                        }),
                        ()=>new DeliveryLocation { Id=it.Id},
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
