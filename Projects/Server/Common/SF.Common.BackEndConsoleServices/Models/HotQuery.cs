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
using System.ComponentModel.DataAnnotations;
using SF.Sys.Auth;

namespace SF.Sys.BackEndConsole.Models
{
	/// <summary>
	/// 常用查询
	/// </summary>
	[EntityObject]
	public class HotQuery : ObjectEntityBase<long>
	{
		/// <summary>
		/// 所有人
		/// </summary>
		[EntityIdent(typeof(User),nameof(OwnerName))]
		public long? OwnerId { get; set; }

		/// <summary>
		/// 所有人
		/// </summary>
		[TableVisible]
		[Ignore]
		public string OwnerName { get; set; }
		
		/// <summary>
		/// 控制台ID
		/// </summary>
		[EntityIdent(typeof(Console))]
		public long ConsoleId { get; set; }

		/// <summary>
		/// 页面
		/// </summary>
		public string PageId { get; set; }

		/// <summary>
		/// 内容
		/// </summary>
		public string ContentId { get; set; }

		/// <summary>
		/// 查询
		/// </summary>
		[MultipleLines]
		public string Query { get; set; }

	}

}

