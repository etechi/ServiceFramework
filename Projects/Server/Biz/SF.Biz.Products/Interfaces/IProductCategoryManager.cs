﻿#region Apache License Version 2.0
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

using SF.Sys.Annotations;
using SF.Sys.Auth;
using SF.Sys.Entities;
using SF.Sys.NetworkService;

namespace SF.Biz.Products
{
	public class CategoryQueryArgument : ObjectQueryArgument<ObjectKey<long>>
    {

		/// <summary>
		/// 卖家
		/// </summary>
		[EntityIdent(typeof(User))]
        [Ignore]
        public long? SellerId { get; set; }

		/// <summary>
		/// 父目录
		/// </summary>
		[EntityIdent(typeof(CategoryInternal))]
        public long? ParentId { get; set; }

	}


	/// <summary>
	/// 产品目录管理
	/// </summary>
	[NetworkService]
	[EntityManager]
	public interface IProductCategoryManager :
		IEntityManager<ObjectKey<long>, CategoryInternal>,
		IEntitySource<ObjectKey<long>, CategoryInternal, CategoryQueryArgument>
	{
		//Task<TEditable[]> BatchUpdate(long SellerId, TEditable[] Items);
		//Task UpdateItems(long CategoryId, long[] Items);
  //      Task<long[]> LoadItems(long CategoryId);
	}
}
