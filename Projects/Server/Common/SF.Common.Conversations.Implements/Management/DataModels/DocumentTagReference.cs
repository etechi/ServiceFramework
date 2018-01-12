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

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SF.Sys.Data;

namespace SF.Common.Documents.DataModels
{
	public class DocumentTagReference : DocumentTagReference<DataDocument, DataDocumentAuthor, DataDocumentCategory, DocumentTag, DocumentTagReference>
	{ }

	/// <summary>
	/// 标签引用
	/// </summary>
	/// <typeparam name="TDocument"></typeparam>
	/// <typeparam name="TAuthor"></typeparam>
	/// <typeparam name="TCategory"></typeparam>
	/// <typeparam name="TTag"></typeparam>
	/// <typeparam name="TTagReference"></typeparam>
	[Table("DocumentTagRef")]
    public class DocumentTagReference<TDocument, TAuthor, TCategory, TTag, TTagReference>
		where TDocument : DataDocument<TDocument, TAuthor, TCategory, TTag, TTagReference>
		where TAuthor : DataDocumentAuthor<TDocument, TAuthor, TCategory, TTag, TTagReference>
		where TCategory : DataDocumentCategory<TDocument, TAuthor, TCategory, TTag, TTagReference>
		where TTag : DocumentTag<TDocument, TAuthor, TCategory, TTag, TTagReference>
		where TTagReference : DocumentTagReference<TDocument, TAuthor, TCategory, TTag, TTagReference>
	{
		/// <summary>
		/// 文档ID
		/// </summary>
		[Key]
		[Column(Order =1)]
		public long DocumentId {get; set;}

		/// <summary>
		/// 标签ID
		/// </summary>
		[Key]
		[Column(Order = 2)]
		[Index]
        public long TagId { get; set; }

		[ForeignKey(nameof(DocumentId))]
		public TDocument Document { get; set; }
		[ForeignKey(nameof(TagId))]
		public TTag Tag { get; set; }
	}
}
