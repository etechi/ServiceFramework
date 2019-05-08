using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Biz.Delivery
{
    public interface IDeliveryAddressSnapshotService
	{
		Task<long> GetAddressId(DeliveryAddress Address);
		Task<DeliveryAddressDetail> QueryAddress(long Id);
	}
}
