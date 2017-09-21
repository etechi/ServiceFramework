using SF.Data.Models;
using SF.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Auth.Permissions.Models
{
   
    [EntityObject("用户授权")]
    public class GrantEditable: ObjectEntityBase<long>
    {


		[EntityIdent(typeof(RoleInternal))]
		[Comment(Name = "角色", Description = "管理员的角色，重新登录后生效")]
		public IEnumerable<string> Roles { get; set; }


		[Comment(Name = "授权清单", Description = "注：1.为了方便管理，建议通过角色来设置权限， 2.新建管理员时需要保存以后才能设置权限， 2.权限修改后，下次用户登录时生效")]
		[TableRows]
		[ReadOnly(true)]
		[Ignore]
		public ResourceGrantInternal[] ResGrants { get; set; }


	}
}
