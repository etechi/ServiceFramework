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

using SF.Sys.Annotations;
using SF.Sys.Entities;
using SF.Sys.NetworkService;
using System.Threading.Tasks;

namespace SF.Sys.MenuServices
{
	public class MenuQueryArgument : Entities.QueryArgument<ObjectKey<long>>
	{
		/// <summary>
		/// 名称
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// 标识
		/// </summary>
		public string Ident { get; set; }
	}
	/// <summary>
	/// 菜单管理
	/// </summary>
	[NetworkService]
	[EntityManager]
	[Category("系统管理", "系统菜单")]
	public interface IMenuService :
		Entities.IEntitySource<ObjectKey<long>, Models.Menu, MenuQueryArgument>,
		Entities.IEntityManager<ObjectKey<long>, Models.MenuEditable>
	{
		Task<MenuItem[]> GetMenu(string Ident);
	}

}

