using System.Threading.Tasks;
using SF.Sys.Data;
using SF.Sys.Entities;
using System.Linq;
using SF.Sys;
using System.Text;
using System.Collections.Generic;
using SF.Common.Addresses.DataModels;
using System;

namespace SF.Common.Addresses.Management
{
    public class UserAddressManager:
        AutoModifiableEntityManager<ObjectKey<long>, UserAddressInternal, UserAddressInternal, UserAddressQueryArguments, UserAddressInternal, DataModels.DataAddress>,
        IUserAddressManager

    {
        Lazy<ILocationManager> DeliveryLocationManager { get; }
        public UserAddressManager(IEntityServiceContext ServiceContext, Lazy<ILocationManager> DeliveryLocationManager) : base(ServiceContext)
        {
            this.DeliveryLocationManager = DeliveryLocationManager;
        }

        protected override async Task OnUpdateModel(IModifyContext ctx)
        {
            var e = ctx.Editable;
            var m = ctx.Model;

            var province = await DeliveryLocationManager.Value.GetAsync(e.ProvinceId);
            var city = await DeliveryLocationManager.Value.GetAsync(e.CityId);
            var district = await DeliveryLocationManager.Value.GetAsync(e.DistrictId);

            m.LocationName = province.Name + city.Name + district.Name;

            

            if (e.IsDefaultAddress)
            {
                var curs = await ctx.DataContext.Queryable<DataModels.DataAddress>(false)
                    .Where(i => i.OwnerId.Equals(m.OwnerId) && i.IsDefaultAddress)
                    .ToArrayAsync();
                if (curs.Length > 0)
                {
                    foreach (var c in curs)
                    {
                        c.IsDefaultAddress = false;
                        ctx.DataContext.Update(c);
                    }
                }
            }
            else
            {
                if (!await ctx.DataContext
                    .Queryable<DataModels.DataAddress>()
                    .AnyAsync(i => i.OwnerId.Equals(e.OwnerId) && i.Id != m.Id && i.IsDefaultAddress)
                    )
                    e.IsDefaultAddress = true;
            }


            await base.OnUpdateModel(ctx);
        }

    }

}
