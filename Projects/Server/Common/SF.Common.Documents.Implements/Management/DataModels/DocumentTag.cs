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

using System.ComponentModel.DataAnnotations.Schema;
using SF.Sys.Entities.DataModels;
using SF.Sys.Data;
using System.ComponentModel.DataAnnotations;

namespace SF.Common.Documents.DataModels
{
	public class DataDocumentTag : DataDocumentTag<DataDocumentScope, DataDocument, DataDocumentAuthor, DataDocumentCategory, DataDocumentTag, DataDocumentTagReference>
	{ }

	/// <summary>
	/// 文档标签
	/// </summary>
	/// <typeparam name="TScope"></typeparam>
	/// <typeparam name="TDocument"></typeparam>
	/// <typeparam name="TAuthor"></typeparam>
	/// <typeparam name="TCategory"></typeparam>
	/// <typeparam name="TTag"></typeparam>
	/// <typeparam name="TTagReference"></typeparam>
	[Table("DocumentTag")]
    public class DataDocumentTag<TScope,TDocument, TAuthor, TCategory, TTag, TTagReference> :
		DataObjectEntityBase
		where TScope : DataDocumentScope<TScope,TDocument, TAuthor, TCategory, TTag, TTagReference> 
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

		[Index("ScopedName",IsUnique =true,Order =1)]
        public override long? ServiceDataScopeId { get; set; }


		[Index("ScopedName", IsUnique = true, Order = 2)]
        public override string Name{get;set;}

	}
}
