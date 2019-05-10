using SF.Biz.Delivery.Management;
using SF.Sys;
using SF.Sys.Auth;
using SF.Sys.Clients;
using SF.Sys.Data;
using SF.Sys.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
namespace SF.Biz.Delivery
{
    public class UserAddressService:
        IUserAddressService

    {
        IUserDeliveryAddressManager AddressManager { get; }
        IAccessToken AccessToken { get; }
        IDataScope DataScope { get; }
        Lazy<IDeliveryLocationService> DeliveryLocationService { get; }
        public UserAddressService(
            IAccessToken AccessToken, 
            IDataScope DataScope,
            IUserDeliveryAddressManager AddressManager,
            Lazy<IDeliveryLocationService> DeliveryLocationService
            )
        {
            this.AccessToken = AccessToken;
            this.AddressManager = AddressManager;
            this.DataScope = DataScope;
            this.DeliveryLocationService = DeliveryLocationService;
        }

        long EnsureUserId()
            => AccessToken.User.EnsureUserIdent();

        IQueryable<UserAddress> MapToClient(IQueryable<DataModels.DataDeliveryAddress> q)
        {
            return q.Select(a => new UserAddress
            {
                Address = a.Address,
                ContactName = a.ContactName,
                ContactPhoneNumber = a.ContactPhoneNumber,
                Id = a.Id,
                LocationId = a.LocationId,
                LocationName = a.LocationName,
                ZipCode = a.ZipCode,
                PhoneNumberVerified=a.PhoneNumberVerified
                
            });
        }
        public async Task<UserAddress> GetUserAddress(long? AddressId = null)
        {
            var uid = EnsureUserId();
            return await DataScope.Use("获取用户地址", ctx =>
            {
                var q = ctx.Queryable<DataModels.DataDeliveryAddress>();
                if (AddressId.HasValue)
                    q = q.Where(a => a.UserId == uid && a.IsDefaultAddress);
                else
                    q = q.Where(a => a.Id == AddressId.Value);
                return MapToClient(q).SingleOrDefaultAsync();
            });
        }
        public async Task<UserAddress[]> ListUserAddresses()
        {
            var uid = EnsureUserId();
            return await DataScope.Use("获取用户地址", ctx =>
            {
                var q = ctx.Queryable<DataModels.DataDeliveryAddress>()
                .Where(a => a.UserId == uid)
                .OrderBy(a => a.CreatedTime);
                return MapToClient(q).ToArrayAsync();
            });
        }

        public async Task<UserAddressEditable> LoadForEditAsync(long AddressId)
        {
            var uid = EnsureUserId();
            return await DataScope.Use("获取用户地址", async ctx =>
            {
                var q = ctx.Queryable<DataModels.DataDeliveryAddress>();
                q = q.Where(a => a.Id == AddressId && a.UserId == uid);
                var addr = await q.Select(a => new
                {
                    editable = new UserAddressEditable
                    {
                        Address = a.Address,
                        Id = a.Id,
                        ContactName = a.ContactName,
                        ContactPhoneNumber = a.ContactPhoneNumber,
                        IsDefaultAddress = a.IsDefaultAddress
                    },
                    locId = a.LocationId
                }).SingleOrDefaultAsync();

                if (addr == null)
                    return null;

                var ls = await DeliveryLocationService.Value.GetPath(addr.locId);
                if (ls.Length >= 4) addr.editable.DistrictId = ls[3].Id;
                if(ls.Length>=3) addr.editable.CityId = ls[2].Id;
                if(ls.Length>=2) addr.editable.ProvinceId = ls[1].Id;

                return addr.editable;
            });
        }

        public async Task RemoveAddress(long AddressId)
        {
            var uid = EnsureUserId();
            var a = await AddressManager.GetAsync(AddressId);
            if (a == null) return;
            if (a.UserId != uid)
                throw new PublicDeniedException();
            await AddressManager.RemoveAsync(AddressId);
        }

        public async Task<long> UpdateAddress(UserAddressEditable address)
        {
            var uid = EnsureUserId();
            var loc = await DeliveryLocationService.Value.Get(address.DistrictId);
            if (loc == null)
                throw new PublicArgumentException("找不到区/县:" + address.DistrictId);
            var re=await AddressManager.EnsureEntity(
                ObjectKey.From(address.Id),
                () => new UserDeliveryAddress { UserId = uid },
                e =>
                {
                    if (e.UserId != uid) throw new PublicDeniedException();
                    e.Address = address.Address;
                    e.CityId = address.CityId;
                    e.ContactName = address.ContactName;
                    e.ContactPhoneNumber = address.ContactPhoneNumber;
                    e.DistrictId = address.DistrictId;
                    e.IsDefaultAddress = address.IsDefaultAddress;
                    e.LocationName = loc.FullName;
                    e.Name = address.Address;
                    e.UserId = uid;
                    e.ProvinceId = address.ProvinceId;
                }
                );
            return re.Id;
        }
    }
}
