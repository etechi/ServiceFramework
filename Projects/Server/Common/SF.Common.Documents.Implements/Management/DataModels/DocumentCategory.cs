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
using SF.Sys.Entities.DataModels;

namespace SF.Common.Documents.DataModels
{
	public class DataDocumentCategory : DataDocumentCategory<DataDocument, DataDocumentAuthor, DataDocumentCategory, DocumentTag, DocumentTagReference>
	{ }

	/// <summary>
	/// 文档目录
	/// </summary>
	/// <typeparam name="TDocument"></typeparam>
	/// <typeparam name="TAuthor"></typeparam>
	/// <typeparam name="TCategory"></typeparam>
	/// <typeparam name="TTag"></typeparam>
	/// <typeparam name="TTagReference"></typeparam>
	[Table("DocumentCategory")]
    public class DataDocumentCategory<TDocument, TAuthor, TCategory, TTag, TTagReference> :
		DataUITreeContainerEntityBase<TCategory,TDocument>
		where TDocument : DataDocument<TDocument, TAuthor, TCategory, TTag, TTagReference>
		where TAuthor : DataDocumentAuthor<TDocument, TAuthor, TCategory, TTag, TTagReference>
		where TCategory : DataDocumentCategory<TDocument, TAuthor, TCategory, TTag, TTagReference>
		where TTag : DocumentTag<TDocument, TAuthor, TCategory, TTag, TTagReference>
		where TTagReference : DocumentTagReference<TDocument, TAuthor, TCategory, TTag, TTagReference>
	{

	}
}
