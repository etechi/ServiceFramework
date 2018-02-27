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

using SF.Sys.Entities.Models;
using SF.Sys.Annotations;
using System;
using System.ComponentModel.DataAnnotations;

namespace SF.Common.Documents.Management
{
	[EntityObject]
	public class CategoryInternal : UITreeContainerEntityBase<CategoryInternal,DocumentInternal>
	{
		/// <summary>
		/// 父分类
		/// </summary>
		[EntityIdent(typeof(Category), nameof(ContainerName), IsTreeParentId = true)]
		[Layout(1, 2)]
		public override long? ContainerId { get; set; }

		/// <summary>
		/// 父分类
		/// </summary>
		public override string ContainerName { get; set; }

		/// <summary>
		/// 文档区域
		/// </summary>
		[EntityIdent(typeof(DocumentScope),nameof(ScopeName))]
		public string ScopeId { get; set; }

		[Ignore]
		[TableVisible]
		public string ScopeName { get; set; }
	}

	[EntityObject]
	public class DocumentBase : UIItemEntityBase<CategoryInternal>
	{
		/// <summary>
		/// 文档区域
		/// </summary>
		[EntityIdent(typeof(DocumentScope), nameof(ScopeName))]
		public string ScopeId { get; set; }

		[Ignore]
		[TableVisible]
		public string ScopeName { get; set; }


		/// <summary>
		/// 文档分类
		/// </summary>
		[EntityIdent(typeof(Category), nameof(ContainerName))]
		[Layout(1, 2)]
		public override long? ContainerId { get; set; }

		/// <summary>
		/// 访问标示
		/// </summary>
		[TableVisible]
		[StringLength(100)]
		public string Ident { get; set; }

		/// <summary>
		/// 发布时间
		/// </summary>
		[TableVisible]
		public DateTime? PublishDate { get; set; }

		/// <summary>
		/// 分类名称
		/// </summary>
		[TableVisible]
		public override string ContainerName { get; set; }

	}
	public class DocumentInternal : DocumentBase
	{
	}
	public class DocumentEditable : DocumentBase
	{
		/// <summary>
		/// 文档内容
		/// </summary>
		[Html]
		public string Content { get; set; }
	}

	public class DocumentScope: ObjectEntityBase<string>
	{
		
	}
}
