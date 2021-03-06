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

using SF.Sys.Annotations;
using SF.Sys.Auth;
using SF.Sys.Entities;
using SF.Sys.NetworkService;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SF.Sys.BackEndConsole.Front
{
	public enum EntityPermissionType
	{
		None,
		ReadOnly,
		Edit,
		NewAndEdit,
		Full
	}
	public class Console
	{
		public long Id { get; set; }
		public string Ident { get; set; }
		public string Title { get; set; }
		public string SystemVersion { get; set; }
		public ConsoleMenuItem[] MenuItems { get; set; }
		public Dictionary<string,EntityPermissionType> EntityPermissions { get; set; }

	}

	
	/// <summary>
	/// 管理控制台服务
	/// </summary>
	[NetworkService]
	[DefaultAuthorize]
	public interface IBackEndAdminConsoleService
	{
		Task<Console> GetConsole(string ConsoleIdent);
		Task<Page> GetPage(string ConsoleIdent, string PagePath);
		Task<bool[]> CheckPermissions(string[] Methods);
		Task HotMenuUpdate(ConsoleMenuItem[] Items);
		Task<ConsoleMenuItem[]> HotMenuList();

	}

}

