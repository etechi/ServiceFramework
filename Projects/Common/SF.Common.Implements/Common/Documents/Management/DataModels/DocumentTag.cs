using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SF.Data;
using SF.Metadata;
using SF.Entities.DataModels;

namespace SF.Common.Documents.DataModels
{
	public class DocumentTag : DocumentTag<Document, DocumentAuthor, DocumentCategory, DocumentTag, DocumentTagReference>
	{ }

	[Table("CommonDocumentTag")]
    [Comment(Name = "文档标签",GroupName ="文档服务")]
    public class DocumentTag<TDocument, TAuthor, TCategory, TTag, TTagReference> :
		ObjectEntityBase
		where TDocument : Document<TDocument, TAuthor, TCategory, TTag, TTagReference>
		where TAuthor : DocumentAuthor<TDocument, TAuthor, TCategory, TTag, TTagReference>
		where TCategory : DocumentCategory<TDocument, TAuthor, TCategory, TTag, TTagReference>
		where TTag : DocumentTag<TDocument, TAuthor, TCategory, TTag, TTagReference>
		where TTagReference : DocumentTagReference<TDocument, TAuthor, TCategory, TTag, TTagReference>
	{
	
		[Index("ScopedName",IsUnique =true,Order =1)]
        public override long? ScopeId { get; set; }


		[Index("ScopedName", IsUnique = true, Order = 2)]
        public override string Name{get;set;}

	}
}
