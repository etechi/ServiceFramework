using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SF.Metadata;
using SF.Entities.DataModels;

namespace SF.Common.Documents.DataModels
{
	[Table("CommonDocumentAuthor")]
	[Comment(GroupName="文档服务", Name = "文档作者")]
    public class DocumentAuthor<TDocument, TAuthor, TCategory, TTag, TTagReference> :
		UIObjectEntityBase
		where TDocument : Document<TDocument, TAuthor, TCategory, TTag, TTagReference>
		where TAuthor : DocumentAuthor<TDocument, TAuthor, TCategory, TTag, TTagReference>
		where TCategory : DocumentCategory<TDocument, TAuthor, TCategory, TTag, TTagReference>
		where TTag : DocumentTag<TDocument, TAuthor, TCategory, TTag, TTagReference>
		where TTagReference : DocumentTagReference<TDocument, TAuthor, TCategory, TTag, TTagReference>
	{

		[InverseProperty(nameof(Document<TDocument, TAuthor, TCategory, TTag, TTagReference>.Author))]
		public ICollection<TDocument> Documents { get; set; }

	}
}
