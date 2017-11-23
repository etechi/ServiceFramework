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

using SF.Entities;
using SF.Metadata;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SF.Biz.Products
{
	[EntityObject]
    public class ItemInternal :
		IEntityWithId<long>,
		IEntityWithName
	{
        [Key]
        [ReadOnly(true)]
        [Comment(Name ="ID")]
        [TableVisible]
		public long Id { get; set; }
        
        [Ignore]
        public long? SourceItemId { get; set; }

        [Ignore]
        [Comment(Name ="产品")]
        [EntityIdent(typeof(ProductInternal),"Title")]
        [TableVisible]
        public long ProductId { get; set; }

        [Comment(Name = "图片")]
        [Image]
        public string Image { get; set; }

        [Comment(Name = "标题")]
        [TableVisible]
        public string Title { get; set; }

        [Comment(Name ="价格")]
        [TableVisible]
        public decimal? Price { get; set; }

        [Comment(Name ="卡密")]
        [TableVisible]
        public bool IsVirtual { get; set; }
		string IEntityWithName.Name { get => Title; set { Title = value; } }
	}
    
	public class ItemEditable :
		IEntityWithId<long>
	{
		public long Id { get; set; }
		public long SellerId { get; set; }
		public long? SourceItemId { get; set; }
		public long ProductId { get; set; }
		public decimal? Price { get; set; }
		public string Title { get; set; }
		public string Image { get; set; }
	}

}
