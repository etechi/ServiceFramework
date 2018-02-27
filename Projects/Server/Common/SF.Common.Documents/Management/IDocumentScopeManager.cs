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
using System.ComponentModel.DataAnnotations;

namespace SF.Common.Documents.Management
{
	public class DocumentScopeQueryArguments : QueryArgument
	{
		/// <summary>
		/// 标题
		/// </summary>
		[StringLength(50)]
		public string Name { get; set; }

	}

	/// <summary>
	/// 文档区域管理
	/// </summary>
	/// <typeparam name="TInternal"></typeparam>
	/// <typeparam name="TEditable"></typeparam>
	[NetworkService]
	[EntityManager]
	public interface IDocumentScopeManager<TInternal, TEditable> :
		IEntitySource<ObjectKey<string>, TInternal, DocumentScopeQueryArguments>,
		IEntityManager<ObjectKey<string>, TEditable>
		where TInternal : DocumentScope
		where TEditable : DocumentScope
	{
	}
	public interface IDocumentScopeManager:
		IDocumentScopeManager<DocumentScope, DocumentScope>
	{

	}

}
