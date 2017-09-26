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
	[EntityObject("Category")]
	public class CategoryInternal : UITreeContainerEntityBase<CategoryInternal,DocumentInternal>
	{
		[EntityIdent(typeof(Category), nameof(ContainerName), IsTreeParentId = true)]
		[Comment("父分类")]
		[Layout(1, 2)]
		public override long? ContainerId { get; set; }

		[Comment(Name = "父分类")]
		public override string ContainerName { get; set; }
	}

	[EntityObject("Document")]
	public class DocumentBase : UIItemEntityBase<CategoryInternal>
	{

		[EntityIdent(typeof(Category), nameof(DocumentInternal.ContainerName))]
		[Comment(Name = "文档分类")]
		[Layout(1, 2)]
		public override long? ContainerId { get => base.ContainerId; set => base.ContainerId = value; }

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
