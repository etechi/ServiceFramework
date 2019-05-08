using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceProtocol.ObjectManager;

using ServiceProtocol.Data.Entity;

namespace ServiceProtocol.Biz.Delivery.Entity
{
	[DataObjectLoader("虚拟项目自动发货规格")]
    public class VIADSpecManager<TUserKey, TVIADSpec, TVIADImportBatch, TVIADImportRecord, TVIADDeliveryRecord>:
		ObjectManager.EntityServiceObjectManager<int, VIADSpec, VIADSpec, TVIADSpec>,
		IVIADSpecManager,
        IDataObjectLoader
		where TUserKey : IEquatable<TUserKey>
		where TVIADSpec : Models.VIADSpec<TUserKey, TVIADSpec, TVIADImportBatch, TVIADImportRecord, TVIADDeliveryRecord>,new()
		where TVIADImportBatch : Models.VIADImportBatch<TUserKey, TVIADSpec, TVIADImportBatch, TVIADImportRecord, TVIADDeliveryRecord>
		where TVIADImportRecord : Models.VIADImportRecord<TUserKey, TVIADSpec, TVIADImportBatch, TVIADImportRecord, TVIADDeliveryRecord>
		where TVIADDeliveryRecord : Models.VIADDeliveryRecord<TUserKey, TVIADSpec, TVIADImportBatch, TVIADImportRecord, TVIADDeliveryRecord>
	{
        protected override async Task<VIADSpec> MapModelToEditable(IContextQueryable<TVIADSpec> Query)
		{
			return await (from s in Query
						  let ss=(from b in s.ImportBatchs
								 group b by 1 into g
								 select new
								 {
									 left=g.Select(gi=>gi.Left).DefaultIfEmpty().Sum(),
									 total=g.Select(gi=>gi.Count).DefaultIfEmpty().Sum()
								 }).FirstOrDefault()
						  select new VIADSpec
						  {
							  Id = s.Id,
							  Name = s.Name,
							  ObjectState = s.ObjectState,
							  TimeCreated = s.TimeCreated,
							  TimeUpdated = s.TimeUpdated,
							  PublicInfoTitle = s.PublicInfoTitle,
							  PrivateInfoTitle = s.PrivateInfoTitle,
							  Help = s.Help,
							  Left = ss == null ? 0 : ss.left,
							  Total = ss == null ? 0 : ss.total
						  }
			).SingleOrDefaultAsync();
		}

        protected override IContextQueryable<VIADSpec> MapModelToInternal(IContextQueryable<TVIADSpec> Query)
		{
			return from s in Query
				   let ss = (from b in s.ImportBatchs
							 group b by 1 into g
							 select new
							 {
								 left = g.Select(gi => gi.Left).DefaultIfEmpty().Sum(),
								 total = g.Select(gi => gi.Count).DefaultIfEmpty().Sum()
							 }).FirstOrDefault()
				   select new VIADSpec
				   {
					   Id = s.Id,
					   Name = s.Name,
					   PublicInfoTitle=s.PublicInfoTitle,
					   PrivateInfoTitle=s.PrivateInfoTitle,
					   Help=s.Help,
					   ObjectState = s.ObjectState,
					   TimeCreated = s.TimeCreated,
					   TimeUpdated = s.TimeUpdated,
					   Left = ss==null?0:ss.left,
					   Total = ss==null?0:ss.total
				   };
		}

		protected override Task OnUpdateModel(ModifyContext ctx)
		{
			var Model = ctx.Model;
			var obj = ctx.Editable;

			Model.Id = obj.Id;
			Model.Name = obj.Name;
			Model.ObjectState = obj.ObjectState;
			Model.TimeUpdated = TimeService.Value.Now;
			Model.PublicInfoTitle = obj.PublicInfoTitle;
			Model.PrivateInfoTitle = obj.PrivateInfoTitle;
			Model.Help = obj.Help;
			return Task.CompletedTask;
		}
		protected override Task OnNewModel(ModifyContext ctx)
		{
			ctx.Model.TimeCreated = TimeService.Value.Now;
			return base.OnNewModel(ctx);
		}

		public async Task<VIADSpec[]> List()
		{
			return await MapModelToInternal(Context.ReadOnly<TVIADSpec>()).ToArrayAsync();
		}

        async Task<IDataObject[]> IDataObjectLoader.Load(string Type, string[][] Keys)
        {
            return await DataObjectLoader.Load(
                Keys,
                id => int.Parse(id[0]),
                id => FindByIdAsync(id),
                async (ids) => {
                    var tmps = await MapModelToInternal(
                        Context.ReadOnly<TVIADSpec>().Where(a => ids.Contains(a.Id))
                        ).ToArrayAsync();
                    return await OnPrepareInternals(tmps);
                }
                );
        }

		Lazy<Times.ITimeService> TimeService { get; }

		public VIADSpecManager(IDataContext context, Lazy<Times.ITimeService> TimeService ,Lazy<IModifyFilter> ModifyFilter) : base(context, ModifyFilter)
		{
			this.TimeService = TimeService;
		}

	}
}
