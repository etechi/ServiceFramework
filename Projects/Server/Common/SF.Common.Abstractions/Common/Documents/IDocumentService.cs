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

using SF.Entities;
using SF.Metadata;
using System.Threading.Tasks;

namespace SF.Common.Documents
{
	[NetworkService]
	[Comment("文档服务")]
	public interface IDocumentService<TDocument, TCategory>:
		IEntityLoadable<ObjectKey<long>,TDocument>,
		IEntityLoadableByKey<TDocument>,
		IContainerLoadable<long,TCategory>,
		IEntitySearchable<TDocument>,
		IContainerItemsListable<long?,TDocument>,
		ITreeContainerListable<long?,TCategory>
		where TDocument : Document
		where TCategory : Category
	{
	}

	public interface IDocumentService : IDocumentService<Document, Category>
	{

	}
}
