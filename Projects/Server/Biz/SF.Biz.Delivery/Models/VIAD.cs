using SF.Sys.Annotations;
using SF.Sys.Auth;
using SF.Sys.Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SF.Biz.Delivery
{
    /// <summary>
    /// 虚拟项目自动发货规格
    /// </summary>
    [EntityObject]
	public class VIADSpec : ObjectEntityBase
	{

        ///<title>公开信息标题</title>
        /// <summary>
        /// 如：卡号,充值编号等。如果不填，则没有公开信息，导入数据的第一列为私密信息。
        /// </summary>

		[MaxLength(40)]
		public string PublicInfoTitle { get; set; }

        ///<title>私密信息标题</title>
        /// <summary>
        /// 如：密码，序列号。必填。
        /// </summary>
		[MaxLength(40)]
		[Required]
		public string PrivateInfoTitle { get; set; }

        ///<title>使用帮助</title>
        /// <summary>
        /// 会随卡密数据一起发送给用户
        /// </summary>
		[MaxLength(100)]
		public string Help { get; set; }

        /// <summary>
        /// 导入条数
        /// </summary>
        [ReadOnly(true)]
		[TableVisible]
		public int Total { get; set; }

        /// <summary>
        /// 剩余条数
        /// </summary>
        [ReadOnly(true)]
		[TableVisible]
		public int Left { get; set; }

		
	}

    /// <summary>
    /// 虚拟项目自动发货导入记录
    /// </summary>
	[EntityObject]
	public class VIADImportRecord : EventEntityBase
	{
        /// <summary>
        /// 批次序号
        /// </summary>
		[ReadOnly(true)]
		[TableVisible(200)]
		[Ignore]
		public int IndexOfBatch { get; set; }

        /// <summary>
        /// 公开信息
        /// </summary>
		[MaxLength(201)]
		[TableVisible(100)]
		public string PublicInfo { get; set; }

        /// <summary>
        /// 私密信息
        /// </summary>
		[MaxLength(200)]
		[Required]
		public string PrivateInfo { get; set; }

		[MaxLength(200)]
		[Display(Name = "附加信息")]
		public string ExtraInfo { get; set; }


	}

	public enum VIADImportBatchState
	{
        /// <summary>
        /// 准备中
        /// </summary>
        Preparing,
        /// <summary>
        /// 发货中
        /// </summary>
        Delivering,
        /// <summary>
        /// 已使用
        /// </summary>
        Deliveried
    }

    /// <summary>
    /// 虚拟项目自动发货导入批次
    /// </summary>
    [EntityObject]
    public class VIADImportBatch : ObjectEntityBase

    { 
        ///规格
        [EntityIdent(typeof(VIADSpec),nameof(SpecName))]
		public long SpecId { get; set; }

        /// <summary>
        /// 规格
        /// </summary>
        [Ignore]
		[TableVisible]
		public string SpecName { get; set; }



        /// <summary>
        /// 批次状态
        /// </summary>
        [ReadOnly(true)]
		[TableVisible]
		public VIADImportBatchState State { get; set; }

        /// <summary>
        /// 总条数
        /// </summary>
        [ReadOnly(true)]
		[TableVisible]
		[Ignore]
		public int Count { get; set; }

        /// <summary>
        /// 剩余条数
        /// </summary>
        [ReadOnly(true)]
		[TableVisible]
		[Ignore]
		public int Left { get; set; }

        /// <summary>
        /// 导入帮助
        /// </summary>
        [ReadOnly(true)]
		[Ignore]
		public string ImportHelp { get; set; }

        /// <summary>
        /// 导入项目
        /// </summary>
		[TableRows]		
		public IEnumerable<VIADImportRecord> Records { get; set; }


        /// <summary>
        /// 导入人
        /// </summary>
        [EntityIdent(typeof(User))]
		[ReadOnly(true)]
		public long OperatorId { get; set; }


	}

	
	public class VIADImportRecordInternal : VIADImportRecord
	{

        /// <summary>
        /// 规格
        /// </summary>
        [EntityIdent(typeof(VIADSpec),nameof(SpecName))]
		public int SpecId { get; set; }

        /// <summary>
        /// 规格
        /// </summary>
        [Ignore]
		[TableVisible(10)]
		public string SpecName { get; set; }


        /// <summary>
        /// 批次
        /// </summary>
		[EntityIdent(typeof(VIADImportBatch),nameof(BatchTitle))]
		public int BatchId { get; set; }

        /// <summary>
        /// 批次
        /// </summary>
        [TableVisible(20)]
		public string BatchTitle { get; set; }


        /// <summary>
        /// 使用时间
        /// </summary>
        public DateTime? DeliveryTime { get; set; }

	}

    /// <summary>
    /// 虚拟项目自动发货记录
    /// </summary>
	[EntityObject]
	public class VIADDeliveryRecord :EventEntityBase
	{

        /// <summary>
        /// 规格
        /// </summary>
        [EntityIdent(typeof(VIADSpec),nameof(SpecName))]
		public long SpecId { get; set; }

        /// <summary>
        /// 规格
        /// </summary>
        [Ignore]
		[TableVisible]
		public string SpecName { get; set; }


        /// <summary>
        /// 批次
        /// </summary>
		[EntityIdent(typeof(VIADImportBatch),nameof(BatchTitle))]
		public long BatchId { get; set; }

        /// <summary>
        /// 批次
        /// </summary>
        [TableVisible]
		public string BatchTitle { get; set; }

        /// <summary>
        /// 批次序号
        /// </summary>
		[ReadOnly(true)]
		[TableVisible]
		public int IndexOfBatch { get; set; }

        /// <summary>
        /// 发货
        /// </summary>
		[Display(Name = "发货ID")]
		[EntityIdent(typeof(DeliveryInternal))]
		public long DeliveryId { get; set; }

        /// <summary>
        /// 发货明细
        /// </summary>
		[EntityIdent(typeof(DeliveryItemInternal),nameof(DeliveryItemName))]
		public int DeliveryItemId { get; set; }

		[Display(Name = "发货明细")]
		[TableVisible]
		[Ignore]
		public string DeliveryItemName { get; set; }


        /// <summary>
        /// 发货内容
        /// </summary>
        [EntityIdent()]
		public string DeliveryPayloadId { get; set; }

        /// <summary>
        /// 发货内容规格
        /// </summary>
        [EntityIdent()]
		public string DeliveryPayloadSpecId { get; set; }

        /// <summary>
        /// 收货用户
        /// </summary>
        [EntityIdent(typeof(User),nameof(UserName))]
		public override long? UserId { get; set; }

        /// <summary>
        /// 收货用户
        /// </summary>
		[Ignore]
		[TableVisible]
		public override string UserName { get; set; }

	}
}
