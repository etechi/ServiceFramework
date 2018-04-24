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

namespace SF.Sys.BackEndConsole.Models
{
	/// <summary>
	/// 控制台
	/// </summary>
	[EntityObject]
	public class Console : ObjectEntityBase<long>
	{
		///<title>控制台引用标识</title>
		/// <summary>
		/// 默认：default
		/// </summary>
		[MaxLength(100)]
		[Required]
		public string Ident { get; set; }
		
	}
	[EntityObject]
	public class ConsoleEditable : Console
	{
		/// <summary>
		/// 页面列表
		/// </summary>
		[JsonData]
		[TreeNodes]
		public Page[] Pages { get; set; }

		/// <summary>
		/// 菜单列表
		/// </summary>
		[JsonData]
		[TreeNodes]
		public ConsoleMenuItem[] Menus { get; set; }
	}
}

