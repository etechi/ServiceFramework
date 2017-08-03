﻿using SF.Data;
using SF.Metadata;
using System.ComponentModel.DataAnnotations;

namespace SF.Management.BizAdmins.Models
{
	public class BizAdminEditable : BizAdminInternal
    {
		[Comment("图标")]
		[Image]
		public virtual string Icon { get; set; }


		[Comment("密码", "新建时必须填写，修改时填写密码将修改会员密码")]
		[Password]
		[MaxLength(100)]
		public virtual string Password { get; set; }

	}
}

