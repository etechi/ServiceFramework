using SF.Entities;
using SF.Metadata;

namespace SF.Common.Documents.Management
{
	public class DocumentCategoryQueryArgument : QueryArgument
	{
		[Comment(Name = "父分类")]
		[EntityIdent("文档目录")]
		public int? ParentId { get; set; }
	}
	[NetworkService]
	public interface IDocumentCategoryManager<TInternal> :
		IEntitySource<long, TInternal, DocumentCategoryQueryArgument>,
		IEntityManager<long, TInternal>
		where TInternal : CategoryInternal
	{
	}
	public interface IDocumentCategoryManager :
		IDocumentCategoryManager<CategoryInternal>
	{

	}
}
