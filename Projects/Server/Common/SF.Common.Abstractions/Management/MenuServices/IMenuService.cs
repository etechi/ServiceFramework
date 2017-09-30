using SF.Entities;
using SF.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Management.MenuServices
{
	public class MenuQueryArgument : Entities.IQueryArgument<ObjectKey<long>>
	{
		public ObjectKey<long> Id { get; set; }
		[Comment("名称")]
		public string Name { get; set; }
		[Comment("标识")]
		public string Ident { get; set; }
	}

	[NetworkService]
	[EntityManager("系统菜单")]
	[Comment("菜单管理")]
	[Category("系统管理", "系统菜单")]
	public interface IMenuService :
		Entities.IEntitySource<ObjectKey<long>, Models.Menu, MenuQueryArgument>,
		Entities.IEntityManager<ObjectKey<long>, Models.MenuEditable>
	{
		Task<Models.MenuItem[]> GetMenu(string Ident);
	}

}

