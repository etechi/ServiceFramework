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
using SF.Sys.Entities;
using SF.Sys.NetworkService;

namespace SF.Biz.Products
{
	public class ProductTypeQueryArgument : IQueryArgument<ObjectKey<long>>
    {
		public ObjectKey<long> Id { get; set; }

		/// <summary>
		/// 对象状态
		/// </summary>
		public EntityLogicState? ObjectState { get; set; }

		/// <summary>
		/// 类型名称
		/// </summary>
		public string Name { get; set; }
    }
	public interface IProductTypeManager: IProductTypeManager<ProductTypeInternal, ProductTypeEditable>
	{

	}

	/// <summary>
	/// 产品类型管理
	/// </summary>
	/// <typeparam name="TInternal"></typeparam>
	/// <typeparam name="TEditable"></typeparam>
	[NetworkService]
	[EntityManager]
	public interface IProductTypeManager<TInternal, TEditable> :
		IEntityManager<ObjectKey<long>, TEditable>,
		IEntitySource<ObjectKey<long>, TInternal,ProductTypeQueryArgument>
		where TInternal : ProductTypeInternal
		where TEditable : ProductTypeEditable
	{
	}
}
