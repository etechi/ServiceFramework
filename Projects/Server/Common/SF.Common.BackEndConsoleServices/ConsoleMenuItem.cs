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

using System.Collections.Generic;

namespace SF.Sys.BackEndConsole
{
	public class ConsoleMenuItem
	{
		/// <summary>
		/// 图标
		/// </summary>
		public string FontIcon { get; set; }
		/// <summary>
		/// 标题
		/// </summary>
		public string Title { get; set; }
		/// <summary>
		/// 链接
		/// </summary>
		public string Link { get; set; }

		/// <summary>
		/// 权限
		/// </summary>
		public string Permission { get; set; }

		/// <summary>
		/// 子菜单
		/// </summary>
		public List<ConsoleMenuItem> Children { get; set; }
	}

}

