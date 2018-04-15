#region Apache License Version 2.0
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
using SF.Sys.Entities.DataModels;
using System.ComponentModel.DataAnnotations.Schema;
using SF.Sys.Data;
using System.ComponentModel.DataAnnotations;

namespace SF.Common.Documents.DataModels
{
	public class DataDocument : DataDocument<DataDocumentScope,DataDocument, DataDocumentAuthor, DataDocumentCategory, DataDocumentTag, DataDocumentTagReference>
	{ }

	/// <summary>
	/// 文档
	/// </summary>
	/// <typeparam name="TScope"></typeparam>
	/// <typeparam name="TDocument"></typeparam>
	/// <typeparam name="TAuthor"></typeparam>
	/// <typeparam name="TCategory"></typeparam>
	/// <typeparam name="TTag"></typeparam>
	/// <typeparam name="TTagReference"></typeparam>
	[Table("Document")]
	public class DataDocument<TScope,TDocument, TAuthor, TCategory, TTag, TTagReference> :
		DataUIItemEntityBase<TCategory>
		where TScope : DataDocumentScope<TScope, TDocument, TAuthor, TCategory, TTag, TTagReference>
		where TDocument : DataDocument<TScope, TDocument, TAuthor, TCategory, TTag, TTagReference>
		where TAuthor : DataDocumentAuthor<TScope, TDocument, TAuthor, TCategory, TTag, TTagReference>
		where TCategory : DataDocumentCategory<TScope, TDocument, TAuthor, TCategory, TTag, TTagReference>
		where TTag : DataDocumentTag<TScope, TDocument, TAuthor, TCategory, TTag, TTagReference>
		where TTagReference : DataDocumentTagReference<TScope, TDocument, TAuthor, TCategory, TTag, TTagReference>
	{
		/// <summary>
		/// 区域
		/// </summary>
		[Required]
		[MaxLength(100)]
		[Index]
		public string ScopeId { get; set; }
		
		[ForeignKey(nameof(ScopeId))]
		public TScope Scope { get; set; }


		/// <summary>
		/// 标识
		/// </summary>
		[Index]
		[MaxLength(50)]
        public string Ident { get; set; }

		/// <summary>
		/// 目录ID
		/// </summary>
		[Index("CategoryPublishDate", Order = 1)]
		public override long? ContainerId { get => base.ContainerId; set => base.ContainerId = value; }

		/// <summary>
		/// 发布时间
		/// </summary>
		[Index("CategoryPublishDate", Order = 2)]
		[Index("PublishDate")]
        public DateTime? PublishDate { get; set; }

		/// <summary>
		/// 重定向
		/// </summary>
		public string Redirect { get; set; }


		/// <summary>
		/// 内容
		/// </summary>
		public string Content { get; set; }

		/// <summary>
		/// 作者ID
		/// </summary>
		[Index]
        public long? AuthorId { get; set; }

		[ForeignKey(nameof(AuthorId))]
		public TAuthor Author { get; set; }

		[InverseProperty(nameof(DataDocumentTagReference<TScope,TDocument, TAuthor, TCategory, TTag, TTagReference>.Document))]
		public ICollection<TTagReference> Tags { get; set; }
		
	}
}
