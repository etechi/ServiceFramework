﻿#region Apache License Version 2.0
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
	public class DocumentTagReference : DocumentTagReference<Document, DocumentAuthor, DocumentCategory, DocumentTag, DocumentTagReference>
	{ }

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
