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


using SF.Sys.Entities.Models;
using SF.Sys.Annotations;
using System.Collections.Generic;

namespace SF.Common.Documents
{
	/// <summary>
	/// 文档实体
	/// </summary>
	[EntityObject]
	public class Document : UIItemEntityBase<Category>
	{
		/// <summary>
		/// 重定向
		/// </summary>
		public string Redirect { get; set; }

		/// <summary>
		/// 文档内容,Html格式
		/// </summary>
		[Html]
		public string Content { get; set; }

		
	}

	/// <summary>
	/// 文档目录实体
	/// </summary>
	[EntityObject]
	public class Category : UITreeContainerEntityBase<Category,Document>
	{
		/// <summary>
		/// 子目录
		/// </summary>
		public override IEnumerable<Category> Children { get; set; }

		/// <summary>
		/// 目录中的文档
		/// </summary>
		public override IEnumerable<Document> Items { get; set; }
	}
}
