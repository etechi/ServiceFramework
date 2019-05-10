using SF.Biz.Delivery.Data;
using SF.Biz.Delivery.Management;
using SF.Sys.Collections.Generic;
using SF.Sys.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Biz.Delivery.Init
{
    public static class Setup
	{
		public static async Task EnsureTransport(this IDataScope DataScope)
		{
            await DataScope.Use("初始化快递公司数据", async context =>
            {
                var items = DeliveryTransportData.Items;
                var count = await context.Queryable<DataModels.DataDeliveryTransport>().CountAsync();
                if (count == items.Length)
                    return;

                var dics = await context.Queryable<DataModels.DataDeliveryTransport>().ToDictionaryAsync(m => m.Code);
                foreach (var it in items)
                {
                    var o = dics.Get(it.Code);
                    var is_new = o == null;
                    if (is_new) o = new DataModels.DataDeliveryTransport();

                    o.Order = it.Order;
                    o.Name = it.Name;
                    o.Site = it.Site;
                    o.ContactName = it.ContactName;
                    o.ContactPhone = it.ContactPhone;
                    o.Code = it.Code;

                    if (is_new)
                        context.Add(o);
                    else
                        context.Update(o);
                }
                await context.SaveChangesAsync();
            });
		}
		public static async Task EnsureLocations(this IDataScope DataScope)
		{
            await DataScope.Use("初始化地区数据", async context =>
            {
                var items = DeliveryLocationData.Locations;
                var count = await context.Queryable<DataModels.DataDeliveryLocation>().CountAsync();
                if (count == items.Length)
                    return;

                var dics = await context.Queryable<DataModels.DataDeliveryLocation>().ToDictionaryAsync(m => m.Id);
                foreach (var it in items)
                {
                    var o = dics.Get(it.Id);
                    var is_new = o == null;
                    if (is_new) o = new DataModels.DataDeliveryLocation();

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

                    if (is_new)
                        context.Add(o);
                    else
                        context.Update(o);
                }
                await context.SaveChangesAsync();
            });
		}
	}
}
