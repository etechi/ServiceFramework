using SF.Auth;
using SF.Auth.Identities.Models;
using SF.Management.BizAdmins.Models;
using SF.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Management.BizAdmins
{
	public class BizAdminQueryArgument : Data.Entity.IQueryArgument<long>
	{
		public Option<long> Id { get; set; }
		public string Account { get; set; }
		public string Name { get; set; }
	}

	[EntityManager("业务管理员")]
	[Authorize("bizadmin")]
	[NetworkService]
	[Comment("业务管理员")]
	[Category("系统管理", "业务管理员管理")]
	public interface IBizAdminManagementService : 
		Data.Entity.IEntitySource<long,BizAdminInternal,BizAdminQueryArgument>,
		Data.Entity.IEntityManager<long,BizAdminEditable>
    {
    }

}

