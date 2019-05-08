using SF.Sys.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using SF.Sys.Annotations;
using SF.Sys.Auth;

namespace SF.Biz.Delivery
{

    public interface IVIADSpecManager :
        IEntityManager<long,VIADSpec>,
        IEntitySource<long,VIADSpec>
	{
	}

	public class VIADImportBatchQueryArgument : EventQueryArgument
    {

        /// <summary>
        /// 规格
        /// </summary>
        [EntityIdent(typeof(VIADSpec))]
		public long? SpecId { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public VIADImportBatchState? State { get; set; }

      
	}


	public class VIADImportRecordQueryArgument :EventQueryArgument
	{
        /// <summary>
        /// 规格
        /// </summary>
		[EntityIdent(typeof(VIADSpec))]
		public long? SpecId { get; set; }

        /// <summary>
        /// 批次
        /// </summary>
		[EntityIdent(typeof(VIADImportBatch))]
		[Display(Name = "")]
		public int? BatchId { get; set; }

	}
	public class BatchImportArgument
	{
        /// <summary>
        /// 规格
        /// </summary>
        [EntityIdent(typeof(VIADSpec))]
		public long SpecId { get; set; }

        ///<title>导入批次标题</title>
        /// <summary>
        /// 仅用于管理
        /// </summary>
		[MaxLength(100)]
		[Required]
		public string Name { get; set; }

        ///<title>导入内容</title>
        /// <summary>
        /// 每行一条记录，如果规格有卡号等公开信息，至少需要2列，如果没有，至少需要1列。每列之间以空白，逗号或分号分割。可以从excel等文档中直接复制粘贴。
        /// </summary>
		[Required]
		[MultipleLines]
		public string Content { get; set; }

		[Ignore]
		public long OperatorId { get; set; }
	}
	public interface IVIADImportBatchManager :
        IEntityManager<long, VIADImportBatch>,
        IEntitySource<long, VIADImportBatch, VIADImportBatchQueryArgument>
    {
		Task StartDelivery(long Id);
		Task<int> Import(BatchImportArgument Arg);
		Task<VIADImportRecordInternal> GetImportRecord(int Id);
		Task<QueryResult<VIADImportRecordInternal>> QueryImportRecord(VIADImportRecordQueryArgument Arg, Paging paging);
	}

	public class VIADDeliveryRecordQueryArgument : EventQueryArgument
	{
        /// <summary>
        /// 规格
        /// </summary>
		[EntityIdent(typeof(VIADSpec))]
		public int? SpecId { get; set; }

        /// <summary>
        /// 批次
        /// </summary>
		[EntityIdent(typeof(VIADImportBatch))]
		public int? BatchId { get; set; }

        /// <summary>
        /// 用户
        /// </summary>
        [EntityIdent(typeof(User))]
		public long UserId { get; set; }
	}


	public class VIADDeliveryArgument
	{
		public long SpecId { get; set; }
		public long UserId { get; set; }
		public long DeliveryId { get; set; }
		public long DeliveryItemId { get; set; }
		public string DeliveryItemName { get; set; }
		public string PayloadId { get; set; }
		public string PayloadSpecId { get; set; }
		public DateTime Time { get; set; }
	}

	public interface IVIADDeliveryService
	{
		Task<Tuple<long,string>> Delivery(VIADDeliveryArgument Arg);
		Task<VIADDeliveryRecord> GetDeliveryRecord(long Id);
		Task<QueryResult<VIADDeliveryRecord>> QueryDeliveryRecord(VIADDeliveryRecordQueryArgument Arg);
	}
}
