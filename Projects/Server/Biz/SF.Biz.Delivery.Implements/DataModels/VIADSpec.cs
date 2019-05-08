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
	
	[Table("app_biz_delivery_viad_spec")]
    [TypeDisplay(GroupName = "发货服务", Name = "虚拟商品自动发货规格")]
    public class VIADSpec<TUserKey, TVIADSpec, TVIADImportBatch, TVIADImportRecord, TVIADDeliveryRecord>:
		IObjectWithId<int>
		where TUserKey : IEquatable<TUserKey>
		where TVIADSpec : VIADSpec<TUserKey, TVIADSpec, TVIADImportBatch, TVIADImportRecord, TVIADDeliveryRecord>
		where TVIADImportBatch : VIADImportBatch<TUserKey, TVIADSpec, TVIADImportBatch, TVIADImportRecord, TVIADDeliveryRecord>
		where TVIADImportRecord : VIADImportRecord<TUserKey, TVIADSpec, TVIADImportBatch, TVIADImportRecord, TVIADDeliveryRecord>
		where TVIADDeliveryRecord : VIADDeliveryRecord<TUserKey, TVIADSpec, TVIADImportBatch, TVIADImportRecord, TVIADDeliveryRecord>
	{
		[Key]
        [Display(Name="ID")]
		public int Id { get; set; }

		[Required]
		[MaxLength(200)]
        [Display(Name = "名称")]
        public string Name { get; set; }

		[Display(Name = "对象状态")]
		public LogicObjectState ObjectState { get; set; }

		[Display(Name = "更新时间")]
		public DateTime TimeUpdated { get; set; }

		[Display(Name = "创建时间")]
		public DateTime TimeCreated { get; set; }

		[Display(Name="公开信息标题",Description ="如：卡号,充值编号等。如果不填，则没有公开信息，导入数据的第一列为私密信息。")]
		[MaxLength(40)]
		public string PublicInfoTitle { get; set; }

		[Display(Name = "私密信息标题", Description = "如：密码，序列号。必填。")]
		[MaxLength(40)]
		[Required]
		public string PrivateInfoTitle { get; set; }

		[Display(Name = "使用帮助", Description = "会随卡密数据一起发送给用户")]
		[MaxLength(100)]
		public string Help { get; set; }

		[InverseProperty(nameof(VIADDeliveryRecord<TUserKey, TVIADSpec, TVIADImportBatch, TVIADImportRecord, TVIADDeliveryRecord>.Spec))]
		public ICollection<TVIADDeliveryRecord> DeliveryRecords { get; set; }

		[InverseProperty(nameof(VIADImportRecord<TUserKey, TVIADSpec, TVIADImportBatch, TVIADImportRecord, TVIADDeliveryRecord>.Spec))]
		public ICollection<TVIADImportRecord> ImportRecords { get; set; }

		[InverseProperty(nameof(VIADImportBatch<TUserKey, TVIADSpec, TVIADImportBatch, TVIADImportRecord, TVIADDeliveryRecord>.Spec))]
		public ICollection<TVIADImportBatch> ImportBatchs{ get; set; }
	}
}
