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

using SF.Data;
using SF.Data.Models;
using SF.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Common.Documents.Management
{
	[EntityObject]
	public class CategoryInternal : UITreeContainerEntityBase<CategoryInternal,DocumentInternal>
	{
		[EntityIdent(typeof(Category), nameof(ContainerName), IsTreeParentId = true)]
		[Comment("父分类")]
		[Layout(1, 2)]
		public override long? ContainerId { get; set; }

		[Comment(Name = "父分类")]
		public override string ContainerName { get; set; }
	}

	[EntityObject]
	public class DocumentBase : UIItemEntityBase<CategoryInternal>
	{

		[EntityIdent(typeof(Category), nameof(ContainerName))]
		[Comment(Name = "文档分类")]
		[Layout(1, 2)]
		public override long? ContainerId { get; set; }

		[TableVisible]
		[Comment(Name = "访问标示")]
		[StringLength(100)]
		public string Ident { get; set; }

		[TableVisible]
		[Comment(Name = "发布时间")]
		public DateTime? PublishDate { get; set; }

		[Display(Name = "分类名称")]
		[TableVisible]
		public override string ContainerName { get; set; }

	}
	public class DocumentInternal : DocumentBase
	{
	}
	public class DocumentEditable : DocumentBase
	{
		[Display(Name = "文档内容")]
		[Html]
		public string Content { get; set; }
	}
}
