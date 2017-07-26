using SF.Auth;
using SF.Auth.Identities.Models;
using SF.Data.Models;
using SF.Metadata;
using System;
using System.ComponentModel.DataAnnotations;

namespace SF.Management.MenuServices.Models
{
	public class Menu : EntityBase<long>
	{
		[MaxLength(100)]
		[Required]
		[Comment("菜单引用标识","约定菜单：业务管理后台:bizadmin,系统管理后台:sysadmin")]
		public string Ident { get; set; }
	}

}

