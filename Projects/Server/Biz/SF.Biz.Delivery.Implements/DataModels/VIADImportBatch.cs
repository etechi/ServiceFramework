using ServiceProtocol.Annotations;
using ServiceProtocol.Biz.Delivery;
using ServiceProtocol.Data.Entity;
using ServiceProtocol.ObjectManager;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProtocol.Biz.Delivery.Entity.Models
{
	
	[Table("app_biz_delivery_viad_import_batch")]
    [TypeDisplay(GroupName = "发货服务", Name = "虚拟商品导入批次记录")]
    public class VIADImportBatch<TUserKey, TVIADSpec,TVIADImportBatch,TVIADImportRecord,TVIADDeliveryRecord> :
		IObjectWithId<int>
		where TUserKey : IEquatable<TUserKey>
		where TVIADSpec: VIADSpec<TUserKey, TVIADSpec, TVIADImportBatch, TVIADImportRecord, TVIADDeliveryRecord>
		where TVIADImportBatch : VIADImportBatch<TUserKey, TVIADSpec, TVIADImportBatch, TVIADImportRecord, TVIADDeliveryRecord>
		where TVIADImportRecord : VIADImportRecord<TUserKey, TVIADSpec, TVIADImportBatch, TVIADImportRecord, TVIADDeliveryRecord>
		where TVIADDeliveryRecord : VIADDeliveryRecord<TUserKey, TVIADSpec, TVIADImportBatch, TVIADImportRecord, TVIADDeliveryRecord>
	{
		[Key]
        [Display(Name="ID")]
		public int Id { get; set; }


		[Index("spec",Order =1)]
		[Display(Name = "规格ID")]
		public int SpecId { get; set; }

		[ForeignKey(nameof(SpecId))]
		public TVIADSpec Spec { get; set; }

		[Display(Name = "导入时间")]
		[Index("spec", Order = 3)]
		public DateTime CreatedTime { get; set; }

		[Display(Name = "启用时间")]
		public DateTime? StartTime { get; set; }

		[Display(Name = "发完时间")]
		public DateTime? CompletedTime { get; set; }

		[Display(Name = "最后更新时间")]
		public DateTime UpdatedTime { get; set; }

		public TUserKey OperatorId { get; set; }

		[Display(Name ="批次名称")]
		[Required]
		[MaxLength(200)]
		public string Title { get; set; }

		[Display(Name ="批次状态")]
		[Index("spec", Order = 2)]
		public VIADImportBatchState State { get; set; }

		[Display(Name = "总条数")]
		public int Count { get; set; }

		[Display(Name = "剩余条数")]
		[Index]
		public int Left { get; set; }

		

		[Display(Name = "乐观锁时间戳")]
		[ConcurrencyCheck]
		[Timestamp]
		public byte[] TimeStamp { get; set; }

		[InverseProperty(nameof(VIADImportRecord<TUserKey, TVIADSpec, TVIADImportBatch, TVIADImportRecord, TVIADDeliveryRecord>.Batch))]
		public ICollection<TVIADImportRecord> Records { get; set; }
	}




}
