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

namespace SF.Management.SysAdmins.Entity.DataModels
{
	[Table("MgrAdmin")]
	public class SysAdmin<TSysAdmin> : DataEntityBase
		where TSysAdmin: SysAdmin<TSysAdmin>
	{

		[Comment("账号")]
		[MaxLength(100)]
		[Index]
		public string Account { get; set; }

		[Comment("图标")]
		[MaxLength(100)]
		public string Icon { get; set; }

	}
}

