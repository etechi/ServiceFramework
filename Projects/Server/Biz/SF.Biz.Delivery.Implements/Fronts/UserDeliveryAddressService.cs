using SF.Sys.Auth;
using SF.Sys.Clients;
using SF.Sys.Data;
using System.Linq;
using System.Threading.Tasks;
namespace SF.Biz.Delivery
{
    public class UserDeliveryAddressService:
        IUserDeliveryAddressService

    {
        TAddressService AddressService { get; }
        IAccessToken AccessToken { get; }
        public UserDeliveryAddressService(IAccessToken AccessToken)
        {
            this.AccessToken = AccessToken;
        }

        long EnsureUserId()
            => AccessToken.User.EnsureUserIdent();

       
        public async Task<UserAddress> GetUserAddress(long? AddressId = null)
        {
            var uid = EnsureUserId();
            if (AddressId == null)
                return await AddressService.GetUserDefaultAddress(uid);
            else
            {
                var addr = await AddressService.FindByIdAsync(AddressId.Value);
                return addr.UserId.Equals(uid) ? addr : null;
            }
        }
        public async Task<UserAddress[]> ListUserAddresses()
        {
            return await this.AddressService.ListUserAddresses(EnsureUserId());
        }

        public async Task<UserAddressEditable> LoadForEditAsync(long AddressId)
        {
            var re = await this.AddressService.LoadForEditAsync(AddressId);
            if (re == null || !re.UserId.Equals(EnsureUserId()))
                return null;
            return re;
        }

        public async Task RemoveAddress(long AddressId)
        {
            var uid = EnsureUserId();

            var addr = await GetUserAddress(AddressId);
            if (addr == null || !addr.UserId.Equals(uid))
                return;

            await this.AddressService.DeleteAsync(AddressId);
        }

        public async Task<long> UpdateAddress(UserAddressEditable address)
        {
            var uid = EnsureUserId();

            if (address.Id == 0)
            {
                address.UserId = uid;
                return await this.AddressService.CreateAsync(address);
            }
            else
            {
                var addr = await GetUserAddress(address.Id);
                if (addr == null || !addr.UserId.Equals(uid))
                    throw new ArgumentException();
                address.UserId = uid;
                await this.AddressService.UpdateAsync(address);
                return address.Id;
            }
        }
    }
}
