using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SF.Metadata;
using SF.Data;

namespace SF.Common.Documents.DataModels
{

	[Table("CommonDocumentTagRef")]
    [Comment(GroupName = "文档服务", Name = "标签引用")]
    public class DocumentTagReference<TDocument, TAuthor, TCategory, TTag, TTagReference>
		where TDocument : Document<TDocument, TAuthor, TCategory, TTag, TTagReference>
		where TAuthor : DocumentAuthor<TDocument, TAuthor, TCategory, TTag, TTagReference>
		where TCategory : DocumentCategory<TDocument, TAuthor, TCategory, TTag, TTagReference>
		where TTag : DocumentTag<TDocument, TAuthor, TCategory, TTag, TTagReference>
		where TTagReference : DocumentTagReference<TDocument, TAuthor, TCategory, TTag, TTagReference>
	{
		[Key]
		[Column(Order =1)]
        [Display(Name ="文档ID")]
		public long DocumentId {get; set;}

		[Key]
		[Column(Order = 2)]
		[Index]
        [Display(Name = "标签ID")]
        public long TagId { get; set; }

		[ForeignKey(nameof(DocumentId))]
		public TDocument Document { get; set; }
		[ForeignKey(nameof(TagId))]
		public TTag Tag { get; set; }
	}
}
