using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceProtocol.ObjectManager;
using ServiceProtocol.Data.Entity;
using ServiceProtocol.DI;

namespace ServiceProtocol.Biz.Delivery.Entity
{
	public static class Setup
	{
		public static async Task EnsureTransport<TModel>(this IDIScope DIScope)
			where TModel : Models.DeliveryTransport, new()
		{
			var context = DIScope.Resolve<IDataContext>();
			var items = Models.DeliveryTransportData.Items;
			var count = await context.ReadOnly<TModel>().CountAsync();
			if (count == items.Length)
				return;

			var dics = await context.ReadOnly<TModel>().ToDictionaryAsync(m => m.Ident);
			foreach (var it in items)
			{
				var o = dics.Get(it.Ident);
				var is_new = o == null;
				if (is_new) o = new TModel();

				o.Order = it.Order;
				o.Name = it.Name;
				o.Site = it.Site;
				o.ContactName = it.ContactName;
				o.ContactPhone = it.ContactPhone;
				o.Ident = it.Ident;

				if (is_new)
					context.Add(o);
				else
					context.Update(o);
			}
			await context.SaveChangesAsync();
		}
		public static async Task EnsureLocations<TModel>(this IDIScope DIScope)
			where TModel : Models.DeliveryLocation,new()
		{
			var context = DIScope.Resolve<IDataContext>();
			var items = Models.DeliveryLocationData.Locations;
			var count = await context.ReadOnly<TModel>().CountAsync();
			if (count == items.Length)
				return;

			var dics = await context.ReadOnly<TModel>().ToDictionaryAsync(m => m.Id);
			foreach(var it in items)
			{
				var o = dics.Get(it.Id);
				var is_new = o == null;
				if (is_new) o = new TModel();

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
				o.Disabled = it.Disabled;

				if (is_new)
					context.Add(o);
				else
					context.Update(o);
			}
			await context.SaveChangesAsync();
		}
	}
}
