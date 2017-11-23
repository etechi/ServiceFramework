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
using SF.Sys.Entities.Annotations;
using SF.Sys.Services.Management.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace SF.Sys.MenuServices
{
	/// <summary>
	/// 菜单项动作
	/// </summary>
	public enum MenuActionType
	{
		/// <summary>
		/// 无
		/// </summary>
		None,
		/// <summary>
		/// 实体管理
		/// </summary>
		EntityManager,
		/// <summary>
		/// 显示表单
		/// </summary>
		Form,
		/// <summary>
		/// 显示列表
		/// </summary>
		List,
		/// <summary>
		/// 显示内嵌网页
		/// </summary>
		IFrame,
		/// <summary>
		/// 打开链接
		/// </summary>
		Link
	}

	public class MenuItem : UIObjectEntityBase<long>
	{

		/// <summary>
		/// 字体图标
		/// </summary>
		[MaxLength(100)]
		public virtual string FontIcon { get; set; }

		/// <summary>
		/// 动作
		/// </summary>
		public MenuActionType Action { get; set; }

		/// <summary>
		/// 动作参数
		/// </summary>
		public string ActionArgument { get; set; }

		/// <summary>
		/// 服务
		/// </summary>
		[EntityIdent(typeof(ServiceInstance))]
		public long? ServiceId { get; set; }

		/// <summary>
		/// 子菜单
		/// </summary>
		[TreeNodes]
		public IEnumerable<MenuItem> Children { get; set; }

		[Ignore]
		public long? ParentId { get; set; }
	}
}