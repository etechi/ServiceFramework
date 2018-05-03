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


using SF.Sys.Annotations;
using SF.Sys.Auth;
using SF.Sys.Entities;
using SF.Sys.NetworkService;
using System.ComponentModel.DataAnnotations;

namespace SF.Common.Documents.Management
{
	public class DocumentCategoryQueryArgument : QueryArgument
	{
		/// <summary>
		 /// 文档区域
		 /// </summary>
		[EntityIdent(typeof(DocumentScope))]
		public string ScopeId { get; set; }


		/// <summary>
		/// 父分类
		/// </summary>
		[EntityIdent(typeof(Category))]
		public long? ParentId { get; set; }

		/// <summary>
		/// 名称
		/// </summary>
		[MaxLength(100)]
		public string Name { get; set; }
	}

	/// <summary>
	/// 分类管理
	/// </summary>
	/// <typeparam name="TInternal"></typeparam>
	[NetworkService]
	[EntityManager]
	[DefaultAuthorize(PredefinedRoles.客服专员, true)]
	[DefaultAuthorize(PredefinedRoles.运营专员)]
	[DefaultAuthorize(PredefinedRoles.系统管理员)]
	public interface IDocumentCategoryManager<TInternal> :
		IEntitySource<ObjectKey<long>, TInternal, DocumentCategoryQueryArgument>,
		IEntityManager<ObjectKey<long>, TInternal>
		where TInternal : CategoryInternal
	{
	}
	public interface IDocumentCategoryManager :
		IDocumentCategoryManager<CategoryInternal>
	{

	}
}
