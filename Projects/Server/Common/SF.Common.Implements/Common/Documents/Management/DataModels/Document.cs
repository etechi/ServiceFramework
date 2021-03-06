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
using SF.Entities.DataModels;
using SF.Data;
using SF.Metadata;

namespace SF.Common.Documents.DataModels
{
	public class Document : Document<Document, DocumentAuthor, DocumentCategory, DocumentTag, DocumentTagReference>
	{ }

	[Table("CommonDocument")]
	[Comment(GroupName = "文档服务", Name = "文档作者")]
	public class Document<TDocument, TAuthor, TCategory, TTag, TTagReference> :
		UIItemEntityBase<TCategory>
		where TDocument : Document<TDocument, TAuthor, TCategory, TTag, TTagReference>
		where TAuthor : DocumentAuthor<TDocument, TAuthor, TCategory, TTag, TTagReference>
		where TCategory : DocumentCategory<TDocument, TAuthor, TCategory, TTag, TTagReference>
		where TTag : DocumentTag<TDocument, TAuthor, TCategory, TTag, TTagReference>
		where TTagReference : DocumentTagReference<TDocument, TAuthor, TCategory, TTag, TTagReference>
	{
		[Index]
		[MaxLength(50)]
        [Display(Name = "标识")]
        public string Ident { get; set; }

		[Index("CategoryPublishDate", Order = 1)]
		[Display(Name = "目录ID")]
		public override long? ContainerId { get => base.ContainerId; set => base.ContainerId = value; }

		[Index("CategoryPublishDate", Order = 2)]
		[Index("PublishDate")]
        [Display(Name = "发布时间")]
        public DateTime? PublishDate { get; set; }


        [Display(Name = "内容")]
        public string Content { get; set; }

		[Index]
        [Display(Name = "作者ID")]
        public long? AuthorId { get; set; }

		[ForeignKey(nameof(AuthorId))]
		public TAuthor Author { get; set; }

		[InverseProperty(nameof(DocumentTagReference<TDocument, TAuthor, TCategory, TTag, TTagReference>.Document))]
		public ICollection<TTagReference> Tags { get; set; }
		
	}
}
