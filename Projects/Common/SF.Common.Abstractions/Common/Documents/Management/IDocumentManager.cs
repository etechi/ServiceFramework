using SF.Entities;
using SF.Metadata;
using System.ComponentModel.DataAnnotations;

namespace SF.Common.Documents.Management
{
	public class DocumentQueryArguments : QueryArgument
	{
		[Comment(Name = "文档分类")]
		[EntityIdent("文档目录")]
		public int? CategoryId { get; set; }

		[Comment(Name = "标题")]
		[StringLength(50)]
		public string Name { get; set; }

		[Comment(Name = "发布日期")]
		public NullableDateQueryRange PublishDate { get; set; }
	}

	[NetworkService]
	public interface IDocumentManager<TInternal, TEditable> :
		IEntitySource<long, TInternal, DocumentQueryArguments>,
		IEntityManager<long, TEditable>
		where TInternal : DocumentInternal
		where TEditable : DocumentEditable
	{
	}
	public interface IDocumentManager:
		IDocumentManager<DocumentInternal,DocumentEditable>
	{

	}

}
