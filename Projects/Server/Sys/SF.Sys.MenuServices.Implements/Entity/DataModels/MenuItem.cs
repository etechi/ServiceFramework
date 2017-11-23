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
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SF.Entities.DataModels;
using SF.Sys.Data;

namespace SF.Sys.MenuServices.Entity.DataModels
{
	[Table("MgrMenuItem")]
	public class MenuItem<TMenu,TMenuItem> : UIObjectEntityBase
		where TMenu : Menu<TMenu, TMenuItem>
		where TMenuItem : MenuItem<TMenu, TMenuItem>

	{

		[MaxLength(100)]
		public string FontIcon{get;set;}

		public MenuActionType Action { get; set; }

		[MaxLength(200)]
		public string ActionArgument { get; set; }

		[Index]
		public long MenuId { get; set; }

		[Index]
		public long? ParentId { get; set; }

		[ForeignKey(nameof(ParentId))]
		public TMenuItem Parent { get; set; }

		[ForeignKey(nameof(MenuId))]
		public TMenu Menu { get; set; }

		[InverseProperty(nameof(Parent))]
		public ICollection<TMenuItem> Children { get; set; }

		/// <summary>
		/// 系统服务实例ID
		/// </summary>
		public long? ServiceId { get; set; }

	}

	public class MenuItem : MenuItem<Menu, MenuItem>
	{ }
}

