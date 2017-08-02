using SF.Data;
using SF.Data.Models;
using SF.Metadata;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace SF.Management.MenuServices.Models
{
	[Comment("菜单项动作")]
	public enum MenuItemAction
	{
		[Comment("无")]
		None,
		[Comment("实体管理")]
		EntityManager,
		[Comment("显示表单")]
		Form,
		[Comment("显示列表")]
		List,
		[Comment("显示内嵌网页")]
		IFrame,
		[Comment("打开链接")]
		Link
	}

	[Comment("菜单项")]
	public class MenuItem : UIEntityBase<long>
	{

		[Comment("字体图标", "字体图标", GroupName = "前端展示")]
		[MaxLength(100)]
		public virtual string FontIcon { get; set; }


		[Comment("动作")]
		public MenuItemAction Action { get; set; }

		[Comment("动作参数")]
		public string ActionArgument { get; set; }

		[Comment("服务")]
		[EntityIdent("系统服务实例")]
		public long? ServiceId { get; set; }

		[Comment("子菜单")]
		[TreeNodes]
		public IEnumerable<MenuItem> Children { get; set; }

		[Ignore]
		public long? ParentId { get; set; }

	}

	public class MenuEditable : Menu
    {
		
		[TreeNodes]
		[Comment("菜单项")]
		public IEnumerable<MenuItem> Items { get; set; }
	}
}

