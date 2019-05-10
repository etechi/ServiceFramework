using SF.Sys.Data;
using SF.Sys.Entities.DataModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SF.Biz.Delivery.DataModels
{

    /// <summary>
    /// 地区
    /// </summary>
    public class DataDeliveryLocation : DataObjectEntityBase
	{
		public DataDeliveryLocation()
		{
		}
		public DataDeliveryLocation(
			int id,
			int ord,
			int lev,
			int c1,
			int c2,
			int c3,
			int c4,
			string name,
			string cname,
			string code,
			string phone_prefix,
			double timezone,
			int currency,
			int lang,
			string fname,
			int priority,
			int? parent,
			bool hidden)
		{
			this.Id = id;
			this.ParentId = parent;
			this.Order = ord;
			this.Level = lev;
			this.L1Code = c1;
			this.L2Code = c2;
			this.L3Code = c3;
			this.L4Code = c4;
			this.Name = cname;
			this.EnName = name;
			this.Code = code;
			this.PhonePrefix = phone_prefix;
			this.TimeZone = timezone;
			this.LogicState = hidden?Sys.Entities.EntityLogicState.Disabled:Sys.Entities.EntityLogicState.Enabled;
		}


        /// <summary>
        /// 一级编号
        /// </summary>
        public int L1Code { get; set; }
        /// <summary>
        /// 二级编号
        /// </summary>
        public int L2Code { get; set; }
        /// <summary>
        /// 三级编号
        /// </summary>
        public int L3Code { get; set; }
        /// <summary>
        /// 四级编号
        /// </summary>
        public int L4Code { get; set; }


        /// <summary>
        /// 父地区
        /// </summary>
		[Index("parent", Order = 1)]
        public long? ParentId { get; set; }

        /// <summary>
        /// 排位
        /// </summary>
		[Index("parent", Order = 2)]
        public int Order { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }

        /// <summary>
        /// 英文名
        /// </summary>
        public string EnName { get; set; }
        /// <summary>
        /// 全名
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// 代码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 电话区号
        /// </summary>
		[Index("phone")]
		[MaxLength(10)]
        public string PhonePrefix { get; set; }

        /// <summary>
        /// 时区
        /// </summary>
        public double TimeZone { get; set; }

        /// <summary>
        /// 级别
        /// </summary>
        public int Level { get; set; }

		[ForeignKey(nameof(ParentId))]
		public DataDeliveryLocation Parent { get; set; }

		[InverseProperty(nameof(Parent))]
		public ICollection<DataDeliveryLocation> Children { get; set; }

	}
}
