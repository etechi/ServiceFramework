using SF.Sys.Data;
using System.Linq;
using System.Threading.Tasks;
namespace SF.Biz.Delivery
{
    public class DeliveryLocationService:
		IDeliveryLocationService
	{
		IDataScope DataScope { get; }
		public DeliveryLocationService(IDataScope DataScope)
        {
			this.DataScope = DataScope;
		}

		IQueryable<Location> MapModelToPublic(IQueryable<DataModels.DataDeliveryLocation> query)
		{
			return from m in query
				   select new Location
				   {
					   Id = m.Id,
					   FullName = m.FullName,
					   Name = m.Name,
					   ParentId = m.ParentId,
					   ZipCode = m.Code,
					   Level = m.Level
				   };
		}

		public async Task<Location> Get(long Id)
		{
            return await DataScope.Use("获取地区", ctx =>
                MapModelToPublic(ctx.Queryable<DataModels.DataDeliveryLocation>().Where(l => l.Id == Id && l.LogicState==Sys.Entities.EntityLogicState.Enabled))
                    .SingleOrDefaultAsync()
                );
		}

		public async Task<Location[]> List(long? ParentId)
		{
			if (ParentId == null)
				ParentId = 37000000;
            return await DataScope.Use("获取地区列表", ctx =>
                MapModelToPublic(
                    ctx.Queryable<DataModels.DataDeliveryLocation>()
                    .Where(l => l.ParentId== ParentId && l.LogicState == Sys.Entities.EntityLogicState.Enabled)
                    .OrderBy(l=>l.Order)
                    )
                    .ToArrayAsync()
                );
        }
	}
}
