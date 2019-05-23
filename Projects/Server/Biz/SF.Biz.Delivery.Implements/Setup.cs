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
        
    }
}
