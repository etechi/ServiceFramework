using System.Threading.Tasks;
using SF.Sys.Entities;

namespace SF.Biz.Delivery.Management
{
    public class DeliveryTransportManager:
        AutoModifiableEntityManager<ObjectKey<long>, DeliveryTransport, DeliveryTransport, DeliveryTransportQueryArguments, DeliveryTransport, DataModels.DataDeliveryTransport>,
        IDeliveryTransportManager
        
    {
        public DeliveryTransportManager(IEntityServiceContext ServiceContext) : base(ServiceContext)
        {
        }

    }

}
