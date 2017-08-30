using SF.Auth;
using SF.Auth.Identities.Models;
using SF.Management.SysAdmins.Models;
using SF.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Management.SysAdmins
{
	public class SysAdminQueryArgument : Entities.IQueryArgument<long>
	{
		public Option<long> Id { get; set; }
		public string Account { get; set; }
		public string Name { get; set; }
	}

	[EntityManager]
	[Authorize("admin")]
	[NetworkService]
	[Comment("系统管理员")]
	[Category("系统管理", "系统管理员管理")]
	public interface ISysAdminManagementService : 
		Entities.IEntitySource<long,SysAdminInternal,SysAdminQueryArgument>,
		Entities.IEntityManager<long,SysAdminEditable>
    {
    }

}

