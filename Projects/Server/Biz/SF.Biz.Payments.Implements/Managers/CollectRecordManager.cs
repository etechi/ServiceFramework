using SF.Sys.Entities;

namespace SF.Biz.Payments.Managers
{

    public class CollectRecordManager :
        AutoQueryableEntitySource<ObjectKey<long>, CollectRecordDetail, CollectRecord, CollectRecordQueryArgument, DataModels.DataRefundRecord>,
        ICollectRecordManager
    {

        public CollectRecordManager(IEntityServiceContext ServiceContext) : base(ServiceContext)
        {
        }

    }
}
