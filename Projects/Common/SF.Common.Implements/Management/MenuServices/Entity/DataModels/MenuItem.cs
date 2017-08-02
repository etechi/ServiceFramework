using SF.Metadata;
using SF.Auth;
using SF.Auth.Identities;
using SF.Users.Members.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.Auth.Identities.Models;
using SF.Data.Entity;
using SF.Data.Storage;
using System.ComponentModel.DataAnnotations.Schema;
using SF.Data;
using System.ComponentModel.DataAnnotations;
using SF.Data.DataModels;
using SF.Management.MenuServices.Models;

namespace SF.Management.MenuServices.Entity.DataModels
{
	[Table("MgrMenuItem")]
	public class MenuItem<TMenu,TMenuItem> : UIObjectEntityBase
		where TMenu : Menu<TMenu, TMenuItem>
		where TMenuItem : MenuItem<TMenu, TMenuItem>

	{

		[MaxLength(100)]
		public string FontIcon{get;set;}

		public MenuItemAction Action { get; set; }

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

		[Comment("系统服务实例ID")]
		public long? ServiceId { get; set; }

	}

	public class MenuItem : MenuItem<Menu, MenuItem>
	{ }
}

