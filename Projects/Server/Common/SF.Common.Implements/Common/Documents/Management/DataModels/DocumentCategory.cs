using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SF.Metadata;
using SF.Entities.DataModels;
using SF.Data;

namespace SF.Common.Documents.DataModels
{
	public class DocumentCategory : DocumentCategory<Document, DocumentAuthor, DocumentCategory, DocumentTag, DocumentTagReference>
	{ }

	[Table("CommonDocumentCategory")]
	[Comment(GroupName ="文档服务", Name = "文档目录")]
    public class DocumentCategory<TDocument, TAuthor, TCategory, TTag, TTagReference> :
		UITreeContainerEntityBase<TCategory,TDocument>
		where TDocument : Document<TDocument, TAuthor, TCategory, TTag, TTagReference>
		where TAuthor : DocumentAuthor<TDocument, TAuthor, TCategory, TTag, TTagReference>
		where TCategory : DocumentCategory<TDocument, TAuthor, TCategory, TTag, TTagReference>
		where TTag : DocumentTag<TDocument, TAuthor, TCategory, TTag, TTagReference>
		where TTagReference : DocumentTagReference<TDocument, TAuthor, TCategory, TTag, TTagReference>
	{

	}
}
