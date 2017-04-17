using SF.System.Auth.Identity.Models;
using SF.Management.SysAdmins.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Management.SysAdmins
{
	public class SysAdminQueryArgument : Data.Entity.IQueryArgument<long>
	{
		public Option<long> Id { get; set; }
		public string NickName { get; set; }
	}
	public interface ISysAdminManagementService : 
		Data.Entity.IEntitySource<long,SysAdminInternal,SysAdminQueryArgument>,
		Data.Entity.IEntityManager<long,SysAdminEditable>
    {
    }

}

