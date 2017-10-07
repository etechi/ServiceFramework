using SF.Entities;
using SF.Metadata;
using System.ComponentModel.DataAnnotations;

namespace SF.Common.Documents.Management
{
	public class DocumentCategoryQueryArgument : QueryArgument
	{
		[Comment(Name = "父分类")]
		[EntityIdent(typeof(Category))]
		public int? ParentId { get; set; }

		[Comment(Name = "名称")]
		[MaxLength(100)]
		public string Name { get; set; }
	}
	[NetworkService]
	[EntityManager]
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
