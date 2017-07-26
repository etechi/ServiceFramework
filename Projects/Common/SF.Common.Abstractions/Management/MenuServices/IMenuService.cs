using SF.Auth.Identities.Models;
using SF.Management.SysAdmins.Models;
using SF.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Management.MenuServices
{
	public class MenuQueryArgument : Data.Entity.IQueryArgument<long>
	{
		public Option<long> Id { get; set; }
		[Comment("名称")]
		public string Name { get; set; }
		[Comment("标识")]
		public string Ident { get; set; }
	}

	[NetworkService]
	public interface IMenuService :
		Data.Entity.IEntitySource<long, Models.Menu, MenuQueryArgument>,
		Data.Entity.IEntityManager<long, Models.MenuEditable>
	{
		Task<Models.MenuItem[]> GetMenu(string Ident);
	}

}

