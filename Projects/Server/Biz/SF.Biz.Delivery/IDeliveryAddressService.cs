using System;
using System.Threading.Tasks;

namespace SF.Biz.Delivery
{
    public interface IDeliveryAddressService
	{
		Task<UserDeliveryAddress> FindByIdAsync(long Id);
		Task<UserDeliveryAddress> GetUserDefaultAddress(long UserId);
		Task<UserDeliveryAddress[]> ListUserAddresses(long UserId);
	}

}
