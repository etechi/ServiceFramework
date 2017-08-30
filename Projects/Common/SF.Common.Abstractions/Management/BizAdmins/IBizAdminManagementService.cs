using SF.Auth;
using SF.Entities;
using SF.Management.BizAdmins.Models;
using SF.Metadata;

namespace SF.Management.BizAdmins
{
	public class BizAdminQueryArgument : QueryArgument
	{
		public string Account { get; set; }
		public string Name { get; set; }
	}

	[EntityManager]
	[Authorize("bizadmin")]
	[NetworkService]
	[Comment("业务管理员")]
	[Category("系统管理", "业务管理员管理")]
	public interface IBizAdminManagementService : 
		IEntitySource<long,BizAdminInternal,BizAdminQueryArgument>,
		IEntityManager<long,BizAdminEditable>
    {
    }

}

