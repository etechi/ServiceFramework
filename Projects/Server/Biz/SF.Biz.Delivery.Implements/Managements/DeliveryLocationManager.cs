using System.Threading.Tasks;
using SF.Sys.Data;
using SF.Sys.Entities;
using System.Linq;
using SF.Sys;
using System.Text;
using System.Collections.Generic;

namespace SF.Biz.Delivery.Management
{
    public class DeliveryLocationManager:
        AutoModifiableEntityManager<ObjectKey<long>, DeliveryLocation, DeliveryLocation, DeliveryLocationQueryArguments, DeliveryLocation, DataModels.DataDeliveryLocation>,
        IDeliveryLocationManager
    {
        public DeliveryLocationManager(IEntityServiceContext ServiceContext) : base(ServiceContext)
        {
        }

    }

}
