#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SF.Sys.Entities;
using SF.Sys.Data;
using SF.Sys.Entities.DataModels;

namespace SF.Biz.Products.Entity.DataModels
{
	public class DataProduct : DataUIObjectEntityBase
    {
		
		/// <summary>
		/// 对象逻辑状态
		/// </summary>
		[Index("all_new",Order=1)]
		[Index("all_price", Order = 1)]
		[Index("all_visited", Order = 1)]
		[Index("all_order", Order = 1)]
		[Index("all_sell", Order = 1)]
		[Index("all_type_new", Order = 1)]
		[Index("all_type_price", Order = 1)]
		[Index("all_type_visited", Order = 1)]
		[Index("all_type_order", Order = 1)]
		[Index("all_type_sell", Order = 1)]        
        public override EntityLogicState LogicState { get; set; }

		[Index("all_type_new", Order = 2)]
		[Index("all_type_price", Order = 2)]
		[Index("all_type_visited", Order = 2)]
		[Index("all_type_order", Order = 2)]
		[Index("all_type_sell", Order = 2)]
		public long TypeId { get; set; }
		[ForeignKey(nameof(TypeId))]

		public DataProductType Type { get; set; }

		///<title>虚拟产品</title>
		/// <summary>
		/// 如卡密
		/// </summary>
        public bool IsVirtual { get; set; }

		/// <summary>
		/// 市场价
		/// </summary>
		public decimal MarketPrice { get; set; }

		/// <summary>
		/// 售价
		/// </summary>
		[Index("all_price", Order = 2)]
		[Index("all_type_price", Order = 3)]
        public decimal Price { get; set; }

		/// <summary>
		/// 禁止使用优惠券
		/// </summary>
		public bool CouponDisabled { get; set; }

		/// <summary>
		/// 访问次数
		/// </summary>
		[Index("all_visited", Order = 2)]
		[Index("all_type_visited", Order = 3)]        
        public int Visited { get; set; }
		/// <summary>
		/// 销售次数
		/// </summary>
		[Index("all_sell", Order = 2)]
		[Index("all_type_sell", Order = 3)]

        public int SellCount { get; set; }

		/// <summary>
		/// 排位
		/// </summary>
		[Index("all_order", Order = 2)]
		[Index("all_type_order", Order = 3)]
        public double Order { get; set; }

		/// <summary>
		/// 上架时间
		/// </summary>
		[Index("all_new", Order = 2)]
		[Index("all_type_new", Order = 3)]
        public DateTime? PublishedTime { get; set; }

       

        [InverseProperty(nameof(DataItem.Product))]
		public ICollection<DataItem> Items { get; set; }

		[InverseProperty(nameof(DataPropertyItem.Product))]
		public ICollection<DataPropertyItem> PropertyItems { get; set; }

		
        [InverseProperty(nameof(DataProductSpec.Product))]
        public ICollection<DataProductSpec> Specs { get; set; }

		/// <summary>
		/// 自动发货规格
		/// </summary>
		public long? VIADSpecId { get; set; }

        /// <summary>
		/// 产品图片
		/// </summary>
		public string Images { get; set; }

        /// <summary>
        /// 产品描述
        /// </summary>
        public string Detail { get; set; }

    }
}
