using System.Threading.Tasks;
using SF.Sys.Data;
using SF.Sys.Entities;
using System.Linq;
using SF.Sys;
using System.Text;
using System.Collections.Generic;

namespace SF.Common.Addresses.Management
{
    public class LocationManager:
        AutoModifiableEntityManager<ObjectKey<int>, LocationInternal, LocationInternal, LocationQueryArguments, LocationInternal, DataModels.DataLocation>,
        ILocationManager
    {
        public LocationManager(IEntityServiceContext ServiceContext) : base(ServiceContext)
        {
        }

    }

}
