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
	public class SysAdminQueryArgument : Data.Entity.IQueryArgument<long>
	{
		public Option<long> Id { get; set; }
		public string Account { get; set; }
		public string Name { get; set; }
	}

	[NetworkService]
	public interface ISysAdminManagementService : 
		Data.Entity.IEntitySource<long,SysAdminInternal,SysAdminQueryArgument>,
		Data.Entity.IEntityManager<long,SysAdminEditable>
    {
    }

}

