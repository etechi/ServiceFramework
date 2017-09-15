using SF.Data;
using SF.Entities.DataModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SF.Management.MenuServices.Entity.DataModels
{
	[Table("MgrMenu")]
	public class Menu<TMenu,TMenuItem> : ObjectEntityBase
		where TMenu : Menu<TMenu, TMenuItem>
		where TMenuItem : MenuItem<TMenu, TMenuItem>
	{
		[Index("ident",Order =1)]
		public override long? ScopeId { get; set; }

		[Index("ident",Order =2)]
		[Required]
		[MaxLength(100)]
		public string Ident { get; set; }


		[InverseProperty(nameof(MenuItem<TMenu, TMenuItem>.Menu))]
		public ICollection<TMenuItem> Items { get; set; }
	}

	public class Menu : Menu<Menu, MenuItem>
	{ }
}

