using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceProtocol.ObjectManager;

using ServiceProtocol.Data.Entity;

namespace ServiceProtocol.Biz.Delivery.Entity
{
	[DataObjectLoader("虚拟项目自动发货导入批次")]
	[DataObjectLoader("虚拟项目自动发货导入记录")]
	public class VIADBatchImportManager<TUserKey, TVIADSpec, TVIADImportBatch, TVIADImportRecord, TVIADDeliveryRecord>:
		ObjectManager.EntityServiceObjectManager<int, VIADImportBatch, VIADImportBatch, TVIADImportBatch>,
		IVIADImportBatchManager,
        IDataObjectLoader
		where TUserKey : IEquatable<TUserKey>
		where TVIADSpec : Models.VIADSpec<TUserKey, TVIADSpec, TVIADImportBatch, TVIADImportRecord, TVIADDeliveryRecord>
		where TVIADImportBatch : Models.VIADImportBatch<TUserKey, TVIADSpec, TVIADImportBatch, TVIADImportRecord, TVIADDeliveryRecord>, new()
		where TVIADImportRecord : Models.VIADImportRecord<TUserKey, TVIADSpec, TVIADImportBatch, TVIADImportRecord, TVIADDeliveryRecord>, new()
		where TVIADDeliveryRecord : Models.VIADDeliveryRecord<TUserKey, TVIADSpec, TVIADImportBatch, TVIADImportRecord, TVIADDeliveryRecord>
	{
		public override string ResourceType => "虚拟项目自动发货导入批次";

		Lazy<Times.ITimeService> TimeService { get;}
		public VIADBatchImportManager(IDataContext context, Lazy<Times.ITimeService> TimeService, Lazy<IModifyFilter> ModifyFilter) : base(context, ModifyFilter)
		{
			this.TimeService = TimeService;
		}
		protected override IContextQueryable<TVIADImportBatch> OnLoadChildObjectsForUpdate(ModifyContext ctx, IContextQueryable<TVIADImportBatch> query)
		{
			query = query.Include(b => b.Records);
			return base.OnLoadChildObjectsForUpdate(ctx, query);
		}
		protected override async Task<VIADImportBatch> MapModelToEditable(IContextQueryable<TVIADImportBatch> Query)
		{
			var q = from s in Query
					select new VIADImportBatch
					{
						Count = s.Count,
						CreatedTime = s.CreatedTime,
						Id = s.Id,
						Left = s.Left,
						OperatorId = s.OperatorId.ToString(),
						SpecId = s.SpecId,
						SpecName = s.Spec.Name,
						Title = s.Title,
						UpdatedTime = s.UpdatedTime,
						State=s.State,
						Records=from r in s.Records select new VIADImportRecord
						{
							Id=r.Id,
							ExtraInfo=r.ExtraInfo,
							IndexOfBatch=r.IndexOfBatch,
							PrivateInfo=r.PrivateInfo,
							PublicInfo=r.PublicInfo
						}
					};

			var re=await q.SingleOrDefaultAsync();
			//var spec = await Context.ReadOnly<TVIADSpec>()
			//	.Where(s => s.Id == re.SpecId)
			//	.Select(s => new { publicTitle = s.PublicInfoTitle, privateTitle = s.PrivateInfoTitle })
			//	.SingleOrDefaultAsync();

			//re.ImportHelp = $"公开信息:{(string.IsNullOrWhiteSpace(spec.publicTitle)?"无":spec.publicTitle)} 私密信息:{spec.privateTitle}";

			return re;
		}

        protected override IContextQueryable<VIADImportBatch> MapModelToInternal(IContextQueryable<TVIADImportBatch> Query)
		{
			return from s in Query
				select new VIADImportBatch
				{
					Count=s.Count,
					CreatedTime=s.CreatedTime,
					Id=s.Id,
					Left=s.Left,
					OperatorId=s.OperatorId.ToString(),
					SpecId=s.SpecId,
					SpecName=s.Spec.Name,
					Title=s.Title,
					State = s.State,
					UpdatedTime =s.UpdatedTime
				};
		}


		IEnumerable<string> SplitLine(string line)
		{
			return from w1 in line.Split(' ')
				   from w2 in w1.Split('\t')
				   from w3 in w2.Split(';')
				   from w4 in w3.Split(',')
				   where w4.Length > 0
				   select w4;
		}
		public async Task<int> Import(BatchImportArgument Arg)
		{
			var spec =await  Context.ReadOnly<TVIADSpec>()
				.Where(s => s.Id == Arg.SpecId)
				.Select(s => new { publicTitle = s.PublicInfoTitle, privateTitle = s.PrivateInfoTitle })
				.SingleOrDefaultAsync();
			if (spec == null)
				throw new PublicArgumentException("找不到指定的规格");

			if(string.IsNullOrWhiteSpace(Arg.Content))
				throw new PublicArgumentException("请输入导入数据");
			var hasPublicTitle = !string.IsNullOrWhiteSpace(spec.publicTitle);
			var lineFields = (from line in Arg.Content.Split('\n').Select((l,row)=>new { text = l, row = row+ 1 })
							  let l = line.text.Trim()
							  where l.Length > 0
							  select new { row = line.row, text=line.text, fields = SplitLine(l).ToArray() }
							).ToArray();

			
			if (hasPublicTitle)
			{
				var errLines = lineFields.Where(l => l.fields.Length < 2).Select(l => l.row).ToArray();
				if (errLines.Length > 0)
					throw new ArgumentNullException("第" + errLines.Join(",") + "行的列数少于2列，请检查数据是否正确。");
			}

			var tooLongErrLines = lineFields.Where(l => 
				l.fields[0].Length>200 ||
				hasPublicTitle && l.fields[1].Length>200
				).Select(l => l.row).ToArray();
			if (tooLongErrLines.Length > 0)
				throw new ArgumentNullException("第" + tooLongErrLines.Join(",") + "行的列宽超过200，请检查数据是否正确。");


			var recs = lineFields.Select(l =>
						new VIADImportRecord
						{
							PublicInfo = hasPublicTitle ? l.fields[0] : null,
							PrivateInfo = hasPublicTitle ? l.fields[1] : l.fields[0],
							ExtraInfo = l.fields.Skip(hasPublicTitle ? 2 : 1).Join(" ").Limit(200)
						}).ToArray();
			return await CreateAsync(new VIADImportBatch
			{
				OperatorId = Arg.OperatorId,
				Title = Arg.Title,
				SpecId = Arg.SpecId,
				Records = recs
			});

		}
		protected override Task OnNewModel(ModifyContext ctx)
		{
			ctx.Model.CreatedTime = TimeService.Value.Now;
			return base.OnNewModel(ctx);
		}

		protected override  Task OnUpdateModel(ModifyContext ctx)
		{
			var Model = ctx.Model;
			var obj = ctx.Editable;

			UIEnsure.HasContent(obj.Title,"请输入批次标题");
			UIEnsure.NotNull(obj.Records, "需要提供导入记录");
			var count = obj.Records.Count();
			UIEnsure.Positive(count, "需要提供导入记录");
			if (Model.State != VIADImportBatchState.Preparing)
				throw new PublicInvalidOperationException("批次已开始发货，不能再进行编辑");

			Model.UpdatedTime = TimeService.Value.Now;
			Model.Title = obj.Title;
			Model.SpecId = obj.SpecId;

			var idx = 1;
			foreach (var r in obj.Records)
			{
				r.IndexOfBatch = idx++;
			}

			Context.Merge(
				Model.Records,
				obj.Records,
				(m, e) => m.Id == e.Id,
				e => new TVIADImportRecord
				{
					SpecId = obj.SpecId,
					IndexOfBatch = e.IndexOfBatch,
					Time = Model.CreatedTime,
					PublicInfo = e.PublicInfo,
					PrivateInfo = e.PrivateInfo
				},
				(m, e) =>
				{
					m.IndexOfBatch = e.IndexOfBatch;
					m.PublicInfo = e.PublicInfo;
					m.PrivateInfo = e.PrivateInfo;
					m.ExtraInfo = e.ExtraInfo;
				}
				);

			Model.Count = count;
			Model.Left = count;
			Model.OperatorId = (TUserKey)Convert.ChangeType(obj.OperatorId, typeof(TUserKey));

			return Task.CompletedTask;
		}
		static PagingQueryBuilder<TVIADImportBatch> pagingBuilder = new PagingQueryBuilder<TVIADImportBatch>(
			"time",
			i => i.Add("time", m => m.CreatedTime,true)
			);
		public async Task<QueryResult<VIADImportBatch>> Query(VIADImportBatchQueryArgument Arg,Paging paging)
		{
			IContextQueryable<TVIADImportBatch> q = Context.ReadOnly<TVIADImportBatch>();
			if (Arg.Id.HasValue)
				q = q.Filter(Arg.Id, r => r.Id);
			else
				q = q.Filter(Arg.SpecId, r => r.SpecId)
					.Filter(Arg.State,r=>r.State)
					.Filter(Arg.Time,r=>r.CreatedTime);
			return await q.ToQueryResultAsync(
				MapModelToInternal,
				OnPrepareInternals,
				pagingBuilder,
				paging
				);
		}
		public async Task StartDelivery(int Id)
		{
			await Context.RetryForConcurrencyException(async () =>
			{
				var b = await Context.Find<TVIADImportBatch>(Id);
				if (b == null)
					throw new PublicArgumentException("找不到指定批次");
				if(b.State!=VIADImportBatchState.Preparing)
					throw new PublicArgumentException("只有准备中的批次才能开始发货");
				b.State = VIADImportBatchState.Delivering;
				b.StartTime = TimeService.Value.Now;
				Context.Update(b);
				await Context.SaveChangesAsync();
			});
		}
		protected override Task OnRemoveModel(ModifyContext ctx)
		{
			if (ctx.Model.State != VIADImportBatchState.Preparing)
				throw new PublicInvalidOperationException("只能删除准备中的批次");
			Context.RemoveRange(ctx.Model.Records);
			return base.OnRemoveModel(ctx);
		}
		async Task<IDataObject[]> IDataObjectLoader.Load(string Type, string[][] Keys)
        {
			if (Type == "虚拟项目自动发货导入批次")
				return await DataObjectLoader.Load(
					Keys,
					id => int.Parse(id[0]),
					id => FindByIdAsync(id),
					async (ids) =>
					{
						var tmps = await MapModelToInternal(
							Context.ReadOnly<TVIADImportBatch>().Where(a => ids.Contains(a.Id))
							).ToArrayAsync();
						return await OnPrepareInternals(tmps);
					}
					);
			else if (Type == "虚拟项目自动发货导入记录")
				return await DataObjectLoader.Load(
			   Keys,
			   id => int.Parse(id[0]),
			   id => GetImportRecord(id),
			   async (ids) =>
			   {
				   var tmps = await MapImportRecord(
					   Context.ReadOnly<TVIADImportRecord>().Where(a => ids.Contains(a.Id))
					   ).ToArrayAsync();
				   return tmps;
			   }
			   );
			else
				return Empty.Array<IDataObject>();
		}

		IContextQueryable<VIADImportRecordInternal> MapImportRecord(IContextQueryable<TVIADImportRecord> query)
		{
			return from r in query
				   select new VIADImportRecordInternal
				   {
					   Id = r.Id,
					   BatchId = r.BatchId,
					   BatchTitle = r.Batch.Title,
					   DeliveryTime = r.DeliveryTime,
					   IndexOfBatch = r.IndexOfBatch,
					   PublicInfo = r.PublicInfo,
					   ExtraInfo=r.ExtraInfo,
					   SpecId = r.SpecId,
					   SpecName = r.Spec.Name,
					   Time = r.Time
				   };
		}

		public async Task<VIADImportRecordInternal> GetImportRecord(int Id)
		{
			var re = await MapImportRecord(Context.ReadOnly<TVIADImportRecord>().Where(r => r.Id == Id)).SingleOrDefaultAsync();
			return re;
		}
		static PagingQueryBuilder<TVIADImportRecord> recordPagingBuilder = new PagingQueryBuilder<TVIADImportRecord>(
			"time",
			i => i.Add("time", m => m.Time, true)
			);
		public async Task<QueryResult<VIADImportRecordInternal>> QueryImportRecord(VIADImportRecordQueryArgument Arg, Paging paging)
		{
			IContextQueryable<TVIADImportRecord> q = Context.ReadOnly<TVIADImportRecord>();
			if (Arg.Id.HasValue)
				q = q.Filter(Arg.Id, r => r.Id);
			else
				q = q.Filter(Arg.SpecId, r => r.SpecId)
					.Filter(Arg.Time, r => r.Time)
					.Filter(Arg.BatchId, r => r.BatchId);

			return await q.ToQueryResultAsync(
				MapImportRecord,
				r=>r,
				recordPagingBuilder,
				paging
				);
		}
	}
}
