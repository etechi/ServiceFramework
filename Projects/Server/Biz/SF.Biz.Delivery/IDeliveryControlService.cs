using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProtocol.Biz.Delivery
{
    public interface IDeliveryControlService
	{
        Task Delivery(long DeliveryId, long OperatorId);
        Task UpdateTransportCode(
            long DeliveryId, 
            long OperatorId,
            long? TransportId,
            string TransportCode,
            string VirtualItemToken
            );
		Task Received(long DeliveryId, long OperatorId);

        Task<string> GetVirtualItemToken(long DeliveryId, long OperatorId);

    }
}
