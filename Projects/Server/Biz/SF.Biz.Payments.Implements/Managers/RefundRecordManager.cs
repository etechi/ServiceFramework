using SF.Biz.Payments.DataModels;
using SF.Sys.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
namespace SF.Biz.Payments.Managers
{
    public class RefundRecordManager:
        AutoQueryableEntitySource<ObjectKey<long>, RefundRecordDetail,RefundRecord, RefundRecordQueryArgument,DataModels.DataRefundRecord>,
        IRefundRecordManager
	{

        public RefundRecordManager(IEntityServiceContext ServiceContext):base(ServiceContext)
		{
        }

    }
}
