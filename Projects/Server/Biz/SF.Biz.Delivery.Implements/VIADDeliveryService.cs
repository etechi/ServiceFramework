using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceProtocol.ObjectManager;

using ServiceProtocol.Data.Entity;

namespace ServiceProtocol.Biz.Delivery.Entity
{
	[DataObjectLoader("虚拟项目自动发货记录")]
	public class VIADDeliverySevice<TUserKey, TVIADSpec, TVIADImportBatch, TVIADImportRecord, TVIADDeliveryRecord>:
		IVIADDeliveryService<TUserKey>,
        IDataObjectLoader
		where TUserKey : IEquatable<TUserKey>
		where TVIADSpec : Models.VIADSpec<TUserKey, TVIADSpec, TVIADImportBatch, TVIADImportRecord, TVIADDeliveryRecord>
		where TVIADImportBatch : Models.VIADImportBatch<TUserKey, TVIADSpec, TVIADImportBatch, TVIADImportRecord, TVIADDeliveryRecord>
		where TVIADImportRecord : Models.VIADImportRecord<TUserKey, TVIADSpec, TVIADImportBatch, TVIADImportRecord, TVIADDeliveryRecord>
		where TVIADDeliveryRecord : Models.VIADDeliveryRecord<TUserKey, TVIADSpec, TVIADImportBatch, TVIADImportRecord, TVIADDeliveryRecord>, new()
	{
		Lazy<Times.ITimeService> TimeService { get;}
		IDataContext Context { get; }
		Lazy<IDataObjectResolver> ObjectResolver { get; }
		public VIADDeliverySevice(IDataContext context, Lazy<Times.ITimeService> TimeService, Lazy<IDataObjectResolver> ObjectResolver) 
		{
			this.TimeService = TimeService;
			this.Context = context;
			this.ObjectResolver = ObjectResolver;
		}
		
        async Task<IDataObject[]> IDataObjectLoader.Load(string Type, string[][] Keys)
        {
			return await DataObjectLoader.Load(
			   Keys,
			   id => int.Parse(id[0]),
			   id => GetDeliveryRecord(id),
			   async (ids) =>
			   {
				   var tmps = await MapDeliveryRecord(
					   Context.ReadOnly<TVIADDeliveryRecord>().Where(a => ids.Contains(a.Id))
					   ).ToArrayAsync();
				   return await PrepareRecords(tmps);
			   }
			   );
		}

		IContextQueryable<VIADDeliveryRecord<TUserKey>> MapDeliveryRecord(IContextQueryable<TVIADDeliveryRecord> query)
		{
			return from r in query
				   select new VIADDeliveryRecord<TUserKey>
				   {
					   BatchId=r.BatchId,
					   BatchTitle =r.Batch.Title,
					   DeliveryId=r.DeliveryId,
					   DeliveryItemId=r.DeliveryItemId,
					   DeliveryItemName=r.DeliveryItemName,
					   DeliveryPayloadId=r.PayloadId,
					   DeliveryPayloadSpecId=r.PayloadSpecId,
					   Id=r.Id,
					   IndexOfBatch=r.IndexOfBatch,
					   SpecId=r.SpecId,
					   SpecName=r.Spec.Name,
					   Time=r.Time,
					   UserId=r.UserId
				   };
		}
		async Task<VIADDeliveryRecord<TUserKey>[]> PrepareRecords(VIADDeliveryRecord<TUserKey>[] recs)
		{
			await ObjectResolver.Value.Fill(
				recs,
				r => "用户-" + r.UserId,
				(r, n) => r.UserName = n
				);
			return recs;
		}
		public async Task<VIADDeliveryRecord<TUserKey>> GetDeliveryRecord(int Id)
		{
			var re = await MapDeliveryRecord(Context.ReadOnly<TVIADDeliveryRecord>().Where(r => r.Id == Id)).SingleOrDefaultAsync();
			if (re != null)
				await PrepareRecords(new[] { re });
			return re;
		}
		static PagingQueryBuilder<TVIADDeliveryRecord> recordPagingBuilder = new PagingQueryBuilder<TVIADDeliveryRecord>(
			"time",
			i => i.Add("time", m => m.Time, true)
			);
		public async Task<QueryResult<VIADDeliveryRecord<TUserKey>>> QueryDeliveryRecord(VIADDeliveryRecordQueryArgument Arg, Paging paging)
		{
			IContextQueryable<TVIADDeliveryRecord> q = Context.ReadOnly<TVIADDeliveryRecord>();
			if (Arg.Id.HasValue)
				q = q.Filter(Arg.Id, r => r.Id);
			else
			{
				q = q.Filter(Arg.SpecId, r => r.SpecId)
					.Filter(Arg.Time, r => r.Time)
					.Filter(Arg.BatchId, r => r.BatchId);
				if (Arg.UserId != null)
				{
					var key = (TUserKey)Convert.ChangeType(Arg.UserId, typeof(TUserKey));
					q = q.Where(r=>r.UserId.Equals(key));
				}
			}

			return await q.ToQueryResultAsync(
				MapDeliveryRecord,
				PrepareRecords,
				recordPagingBuilder,
				paging
				);
		}

		public async Task<Tuple<int,string>> Delivery(VIADDeliveryArgument<TUserKey> Arg)
		{
			var spec = await Context.ReadOnly<TVIADSpec>().Where(s => s.Id == Arg.SpecId).Select(s => new
			{
				state=s.ObjectState,
				publicTitle=s.PublicInfoTitle,
				privateTitle=s.PrivateInfoTitle,
				help=s.Help
			}).SingleOrDefaultAsync();
			if (spec==null)
				throw new InvalidOperationException($"找不到指定自动发货规格: 规格{Arg.SpecId} 发货:{Arg.DeliveryId} 项目:{Arg.DeliveryItemId}");

			if(spec.state!=LogicObjectState.Enabled)
				throw new InvalidOperationException($"指定自动发货规格无效: 规格{Arg.SpecId} 发货:{Arg.DeliveryId} 项目:{Arg.DeliveryItemId}");

			var batch = await Context.Editable<TVIADImportBatch>()
				.Where(b =>
					b.SpecId == Arg.SpecId &&
					b.State==VIADImportBatchState.Delivering
					)
				.OrderBy(b => b.CreatedTime)
				.Take(1)
				.SingleOrDefaultAsync();
			if (batch == null)
				throw new InvalidOperationException($"指定规格的批次没有剩余: 规格{Arg.SpecId} 发货:{Arg.DeliveryId} 项目:{Arg.DeliveryItemId}");


			var rec = await Context.Editable<TVIADImportRecord>()
				.Where(r => r.BatchId == batch.Id && r.IndexOfBatch == batch.Left)
				.SingleOrDefaultAsync();

			if (rec.DeliveryTime.HasValue)
				throw new InvalidOperationException($"代发货项目已经发货，请联系技术人员处理： 规格{Arg.SpecId} 发货:{Arg.DeliveryId} 项目:{Arg.DeliveryItemId}");

			batch.Left--;
			batch.UpdatedTime = Arg.Time;
			if (batch.Left == 0)
			{
				batch.CompletedTime = Arg.Time;
				batch.State = VIADImportBatchState.Deliveried;
			}
			Context.Update(batch);

			rec.DeliveryTime = Arg.Time;
			Context.Update(rec);

			Context.Add(new TVIADDeliveryRecord
			{
				Id = rec.Id,
				SpecId = Arg.SpecId,
				BatchId = batch.Id,
				IndexOfBatch = rec.IndexOfBatch,
				Time = Arg.Time,
				DeliveryId = Arg.DeliveryId,
				DeliveryItemId = Arg.DeliveryItemId,
				PayloadId = Arg.PayloadId,
				PayloadSpecId = Arg.PayloadSpecId,
				UserId = Arg.UserId,
				DeliveryItemName = Arg.DeliveryItemName
			});
			await Context.SaveChangesAsync();
			
			return Tuple.Create(
				rec.Id,
				(string.IsNullOrWhiteSpace(spec.publicTitle)?"": $"{spec.publicTitle}: {rec.PublicInfo??""}\n")+
				$"{spec.privateTitle}: {rec.PrivateInfo}\n"+
				(string.IsNullOrWhiteSpace(spec.help)?"": $"{spec.help}\n")
				); 
		}
	}
}
