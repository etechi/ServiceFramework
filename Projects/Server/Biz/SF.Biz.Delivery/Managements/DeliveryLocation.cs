﻿using SF.Sys.Annotations;
using SF.Sys.Entities.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SF.Biz.Delivery.Management
{
    /// <summary>
    /// 地区
    /// </summary>
    [EntityObject]
    public class DeliveryLocation : ObjectEntityBase
    {
        /// <summary>
        /// 父地区
        /// </summary>
        [EntityIdent(typeof(DeliveryLocation),nameof(ParentName))]
        public long? ParentId { get; set; }

        /// <summary>
        /// 父地区
        /// </summary>
        [Ignore]
        [TableVisible]
        public string ParentName { get; set; }
        /// <summary>
        /// 英文名
        /// </summary>
        [TableVisible]
        public string EnName { get; set; }
        /// <summary>
        /// 全名
        /// </summary>
        [TableVisible]
        public string FullName { get; set; }

        /// <summary>
        /// 代码
        /// </summary>
        [TableVisible]
        public string Code { get; set; }

        /// <summary>
        /// 电话区号
        /// </summary>
        [MaxLength(10)]
        [TableVisible]
        public string PhonePrefix { get; set; }

        /// <summary>
        /// 时区
        /// </summary>
        [TableVisible]
        public double TimeZone { get; set; }

        /// <summary>
        /// 级别
        /// </summary>
        [TableVisible]
        public int Level { get; set; }

        /// <summary>
        /// 排位
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }

        /// <summary>
        /// 一级编号
        /// </summary>
        [ReadOnly(true)]
        public int L1Code { get; set; }
        /// <summary>
        /// 二级编号
        /// </summary>
        [ReadOnly(true)]
        public int L2Code { get; set; }
        /// <summary>
        /// 三级编号
        /// </summary>
        [ReadOnly(true)]
        public int L3Code { get; set; }
        /// <summary>
        /// 四级编号
        /// </summary>
        [ReadOnly(true)]
        public int L4Code { get; set; }

    }
}
