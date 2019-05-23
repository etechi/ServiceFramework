using SF.Common.Addresses.Management;
using SF.Sys;
using SF.Sys.Auth;
using SF.Sys.Clients;
using SF.Sys.Data;
using SF.Sys.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
namespace SF.Common.Addresses
{
    public class UserAddressService:
        IUserAddressService

    {
        IUserAddressManager AddressManager { get; }
        IAccessToken AccessToken { get; }
        IDataScope DataScope { get; }
        Lazy<ILocationService> DeliveryLocationService { get; }
        public UserAddressService(
            IAccessToken AccessToken, 
            IDataScope DataScope,
            IUserAddressManager AddressManager,
            Lazy<ILocationService> DeliveryLocationService
            )
        {
            this.AccessToken = AccessToken;
            this.AddressManager = AddressManager;
            this.DataScope = DataScope;
            this.DeliveryLocationService = DeliveryLocationService;
        }

        long EnsureUserId()
            => AccessToken.User.EnsureUserIdent();

        IQueryable<UserAddress> MapToClient(IQueryable<DataModels.DataAddress> q)
        {
            return q.Select(a => new UserAddress
            {
                Address = a.Address,
                ContactName = a.ContactName,
                ContactPhoneNumber = a.ContactPhoneNumber,
                Id = a.Id,
                LocationId = a.DistrictId,
                LocationName = a.LocationName,
                ZipCode = a.ZipCode,
                PhoneNumberVerified=a.PhoneNumberVerified,
                IsDefaultAddress=a.IsDefaultAddress
                
            });
        }
        public async Task<UserAddress> GetUserAddress(long? AddressId = null)
        {
            var uid = EnsureUserId();
            return await DataScope.Use("获取用户地址", ctx =>
            {
                var q = ctx.Queryable<DataModels.DataAddress>();
                if (AddressId.HasValue)
                    q = q.Where(a => a.Id == AddressId.Value);
                else
                    q = q.Where(a => a.OwnerId == uid && a.IsDefaultAddress);
                    
                return MapToClient(q).SingleOrDefaultAsync();
            });
        }
        public async Task<UserAddress[]> ListUserAddresses()
        {
            var uid = EnsureUserId();
            return await DataScope.Use("获取用户地址", ctx =>
            {
                var q = ctx.Queryable<DataModels.DataAddress>()
                .Where(a => a.OwnerId == uid)
                .OrderBy(a => a.CreatedTime);
                return MapToClient(q).ToArrayAsync();
            });
        }

        public async Task<UserAddressEditable> LoadForEditAsync(long AddressId)
        {
            var uid = EnsureUserId();
            return await DataScope.Use("获取用户地址", async ctx =>
            {
                var q = ctx.Queryable<DataModels.DataAddress>();
                q = q.Where(a => a.Id == AddressId && a.OwnerId == uid);
                var addr = await q.Select(a =>new UserAddressEditable
                    {
                        Address = a.Address,
                        Id = a.Id,
                        ContactName = a.ContactName,
                        ContactPhoneNumber = a.ContactPhoneNumber,
                        IsDefaultAddress = a.IsDefaultAddress,
                        ProvinceId=a.ProvinceId,
                        DistrictId=a.DistrictId,
                        CityId=a.CityId
                    }).SingleOrDefaultAsync();

                if (addr == null)
                    throw new PublicArgumentException("找不到指定的地址");
                return addr;
            });
        }

        public async Task RemoveAddress(long AddressId)
        {
            var uid = EnsureUserId();
            var a = await AddressManager.GetAsync(AddressId);
            if (a == null) return;
            if (a.OwnerId != uid)
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
                () => new Management.UserAddressInternal { OwnerId = uid },
                e =>
                {
                    if (e.OwnerId != uid) throw new PublicDeniedException();
                    
                    e.Address = address.Address;
                    e.CityId = address.CityId;
                    e.ContactName = address.ContactName;
                    e.ContactPhoneNumber = address.ContactPhoneNumber;
                    e.DistrictId = address.DistrictId;
                    e.IsDefaultAddress = address.IsDefaultAddress;
                    e.Name = address.Address;
                    e.ProvinceId = address.ProvinceId;
                }
                );
            return re.Id;
        }
    }
}
