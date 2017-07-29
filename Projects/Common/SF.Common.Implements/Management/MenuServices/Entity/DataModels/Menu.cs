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

namespace SF.Management.MenuServices.Entity.DataModels
{
	[Table("MgrMenu")]
	public class Menu<TMenu,TMenuItem> : DataEntityBase
		where TMenu : Menu<TMenu, TMenuItem>
		where TMenuItem : MenuItem<TMenu, TMenuItem>
	{
		[Index("ident",Order =1)]
		public long? ScopeId { get; set; }

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

