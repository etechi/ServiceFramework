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
	
	[Table("app_biz_delivery_viad_delivery_record")]
    [TypeDisplay(GroupName = "发货服务", Name = "虚拟商品记录")]
    public class VIADDeliveryRecord<TUserKey, TVIADSpec, TVIADImportBatch, TVIADImportRecord, TVIADDeliveryRecord> :
		IObjectWithId<int>
		where TUserKey : IEquatable<TUserKey>
		where TVIADSpec : VIADSpec<TUserKey, TVIADSpec, TVIADImportBatch, TVIADImportRecord, TVIADDeliveryRecord>
		where TVIADImportBatch : VIADImportBatch<TUserKey, TVIADSpec, TVIADImportBatch, TVIADImportRecord, TVIADDeliveryRecord>
		where TVIADImportRecord : VIADImportRecord<TUserKey, TVIADSpec, TVIADImportBatch, TVIADImportRecord, TVIADDeliveryRecord>
		where TVIADDeliveryRecord : VIADDeliveryRecord<TUserKey, TVIADSpec, TVIADImportBatch, TVIADImportRecord, TVIADDeliveryRecord>
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name="ID")]
		[ForeignKey(nameof(ImportRecord))]
		public int Id { get; set; }


		public TVIADImportRecord ImportRecord { get; set; }

		[Index]
		[Display(Name = "规格ID")]
		public int SpecId { get; set; }

		[ForeignKey(nameof(SpecId))]
		public TVIADSpec Spec { get; set; }

		[Index]
		[Display(Name = "批次ID")]
		public int BatchId { get; set; }

		[ForeignKey(nameof(BatchId))]
		public TVIADImportBatch Batch{ get; set; }

		[Display(Name = "批次序号")]
		public int IndexOfBatch { get; set; }

		[Display(Name = "发货时间")]
		public DateTime Time { get; set; }

		[Index]
		[Display(Name = "发货ID")]
		public int DeliveryId { get; set; }

		[Index]
		[Display(Name = "发货明细ID")]
		public int DeliveryItemId { get; set; }

		[Display(Name = "发货项目内容ID")]
		[Required]
		[MaxLength(100)]
		public string PayloadId { get; set; }

		[Display(Name = "发货项目内容规格ID")]
		[MaxLength(100)]
		public string PayloadSpecId { get; set; }

		[Index]
		[Display(Name = "收货用户ID")]
		public TUserKey UserId { get; set; }

		[Display(Name = "发货明细名称")]
		[MaxLength(200)]
		public string DeliveryItemName { get; set; }

	}
}
