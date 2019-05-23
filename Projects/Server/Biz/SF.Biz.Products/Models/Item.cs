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

using SF.Biz.Shops.Managements;
using SF.Sys.Annotations;
using SF.Sys.Entities;
using SF.Sys.Entities.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SF.Biz.Products
{
	[EntityObject]
    public class ItemInternal : UIObjectEntityBase
	{
        /// <summary>
         /// 名称
         /// </summary>
        [MaxLength(100)]
        [EntityTitle]
        [TableVisible]
        public override string Name { get; set; }

        ///<title>标题</title>
        /// <summary>
        /// 用于前端显示
        /// </summary>
        /// <group>前端展示</group>
        [MaxLength(100)]
        [TableVisible]
        public override string Title { get; set; }

        [Ignore]
        public long? SourceItemId { get; set; }

        /// <summary>
        /// 店铺
        /// </summary>
        [EntityIdent(typeof(ShopInternal),nameof(ShopName))]
        public long? ShopId { get; set; }
        
        /// <summary>
        /// 店铺
        /// </summary>
        [Ignore]
        [TableVisible]
        public string ShopName { get; set; }

		/// <summary>
		/// 产品
		/// </summary>
		[Ignore]
        [EntityIdent(typeof(ProductInternal),nameof(ProductName))]
        [TableVisible]
        public long ProductId { get; set; }


        /// <summary>
        /// 产品
        /// </summary>
        public string ProductName { get; set; }
		
		/// <summary>
		/// 价格
		/// </summary>
		[TableVisible]
        public decimal? Price { get; set; }

		/// <summary>
		/// 卡密
		/// </summary>
		[TableVisible]
        public bool IsVirtual { get; set; }
		
	}
    
	public class ItemEditable :
        ItemInternal
	{
		public long SellerId { get; set; }
	}

}
