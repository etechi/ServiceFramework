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
	
	[Table("app_biz_delivery_viad_import_record")]
    [TypeDisplay(GroupName = "发货服务", Name = "虚拟商品导入记录")]
    public class VIADImportRecord<TUserKey, TVIADSpec,TVIADImportBatch,TVIADImportRecord,TVIADDeliveryRecord> :
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


		[Index]
		[Display(Name = "规格ID")]
		public int SpecId { get; set; }

		[ForeignKey(nameof(SpecId))]
		public TVIADSpec Spec { get; set; }


		[Index("batch",Order =1,IsUnique =true)]
		[Display(Name = "批次ID")]
		public int BatchId { get; set; }

		[Index("batch", Order = 2, IsUnique = true)]
		public int IndexOfBatch { get; set; }


		[ForeignKey(nameof(BatchId))]
		public TVIADImportBatch Batch { get; set; }

		[Display(Name = "导入时间")]
		public DateTime Time { get; set; }


		[ForeignKey(nameof(Id))]
		public TVIADDeliveryRecord DeliveryRecord { get; set; }

		[MaxLength(200)]
		[Display(Name ="公开信息")]
		public string PublicInfo { get; set; }

		[MaxLength(200)]
		[Display(Name = "私密信息")]
		public string PrivateInfo { get; set; }

		[MaxLength(200)]
		[Display(Name = "其他信息")]
		public string ExtraInfo { get; set; }

		[Display(Name ="使用时间")]
		public DateTime? DeliveryTime { get; set; }

		[Display(Name = "乐观锁时间戳")]
		[ConcurrencyCheck]
		[Timestamp]
		public byte[] TimeStamp { get; set; }

	}
}
