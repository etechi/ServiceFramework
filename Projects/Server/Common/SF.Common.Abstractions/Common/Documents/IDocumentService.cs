using SF.Entities;
using SF.Metadata;
using System.Threading.Tasks;

namespace SF.Common.Documents
{
	[NetworkService]
	[Comment("文档服务")]
	public interface IDocumentService<TDocument, TCategory>:
		IEntityLoadable<TDocument>,
		IEntityLoadableByKey<TDocument>,
		IContainerLoadable<long,TCategory>,
		IEntitySearchable<TDocument>,
		IContainerItemsListable<long?,TDocument>,
		ITreeContainerListable<long?,TCategory>
		where TDocument : Document
		where TCategory : Category
	{
	}

	public interface IDocumentService : IDocumentService<Document, Category>
	{

	}
}
