using SF.Sys;
using SF.Sys.Data;
using System.Linq;
using System.Threading.Tasks;
namespace SF.Common.Addresses
{
    public class DeliveryLocationService:
		ILocationService
	{
		IDataScope DataScope { get; }
		public DeliveryLocationService(IDataScope DataScope)
        {
			this.DataScope = DataScope;
		}

		IQueryable<Location> MapModelToPublic(IQueryable<DataModels.DataLocation> query)
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

		public async Task<Location> Get(int Id)
		{
            var re = await DataScope.Use("获取地区", ctx =>
                  MapModelToPublic(ctx.Queryable<DataModels.DataLocation>().Where(l => l.Id == Id && l.LogicState == Sys.Entities.EntityLogicState.Enabled))
                      .SingleOrDefaultAsync()
                );
            if (re == null) throw new PublicArgumentException("找不到地区:" + Id);
            return re;

		}

		public async Task<Location[]> List(int? ParentId)
		{
			if (ParentId == null)
				ParentId = 37000000;
            return await DataScope.Use("获取地区列表", ctx =>
                MapModelToPublic(
                    ctx.Queryable<DataModels.DataLocation>()
                    .Where(l => l.ParentId== ParentId && l.LogicState == Sys.Entities.EntityLogicState.Enabled)
                    .OrderBy(l=>l.Order)
                    )
                    .ToArrayAsync()
                );
        }
	}
}
