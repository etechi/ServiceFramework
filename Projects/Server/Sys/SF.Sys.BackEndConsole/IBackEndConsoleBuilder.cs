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
using System.Threading.Tasks;

namespace SF.Sys.BackEndConsole
{
	public class PageContent
	{
		/// <summary>
		/// 内容路径
		/// </summary>
		public string Path { get; set; }
		/// <summary>
		/// 内容类型
		/// </summary>
		public string Type { get; set; }
		/// <summary>
		/// 内容配置
		/// </summary>
		public string Config { get; set; }
		/// <summary>
		/// 内容资源
		/// </summary>
		public string ResAccessRequest { get; set; }
		/// <summary>
		/// 子内容
		/// </summary>
		public PageContent[] Children { get; set; }
	}
	public class Page
	{
		/// <summary>
		/// 页面标题
		/// </summary>
		public string Title { get; set; }
		/// <summary>
		/// 页面路径
		/// </summary>
		public string Path { get; set; }
		/// <summary>
		/// 页面内容
		/// </summary>
		public PageContent Content { get; set; }
	}
	
	public class MenuItemConfig
	{
		/// <summary>
		/// 菜单标题路径
		/// </summary>
		public string Path { get; set; }

		/// <summary>
		/// 字体图标
		/// </summary>
		public string FontIcon { get; set; }

		/// <summary>
		/// 链接
		/// </summary>
		public string Link { get; set; }

		/// <summary>
		/// 权限
		/// </summary>
		public string Permission { get; set; }
	}
	public interface IBackEndConsoleBuildContext
	{
		void AddPage(Page Page);
		void AddMenuItems(params MenuItemConfig[] Items);
		Task AddEntityManager(string MenuPath, long EntityServiceId);
	}
	public interface IBackEndConsoleBuilderCollection
	{
		void AddBuilder(Func<IServiceProvider,IBackEndConsoleBuildContext, Task> Builder, string ConsoleIdent=null);
	}
}