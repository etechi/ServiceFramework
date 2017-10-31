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

using SF.Auth;
using SF.Entities;
using SF.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Biz.Products
{
    public class CategoryQueryArgument : IQueryArgument<ObjectKey<long>>
    {
		public ObjectKey<long> Id { get; set; }

        [Comment(Name = "卖家")]
        [EntityIdent(typeof(User))]
        [Ignore]
        public long? SellerId { get; set; }

        [Comment(Name = "父目录")]
        [EntityIdent(typeof(CategoryInternal))]
        public long? ParentId { get; set; }

        [Comment(Name = "对象状态")]
        public EntityLogicState? ObjectState { get; set; }

		[Comment(Name = "名称")]
		public string Name { get; set; }
	}

	public interface IProductCategoryManager : IProductCategoryManager<CategoryInternal>
	{ }

	[NetworkService]
	[EntityManager]
	[Comment("产品目录管理")]
	public interface IProductCategoryManager<TEditable> :
		IEntityManager<ObjectKey<long>, TEditable>,
		IEntitySource<ObjectKey<long>, TEditable, CategoryQueryArgument>
		where TEditable : CategoryInternal
	{
		//Task<TEditable[]> BatchUpdate(long SellerId, TEditable[] Items);
		//Task UpdateItems(long CategoryId, long[] Items);
  //      Task<long[]> LoadItems(long CategoryId);
	}
}
