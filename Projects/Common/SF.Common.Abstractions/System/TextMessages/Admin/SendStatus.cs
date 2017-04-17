using SF.Data;
using SF.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.System.TextMessages.Admin
{
	public enum SendStatus
	{
		[Display(Name = "发送中")]
		Sending,
		[Display(Name = "发送成功")]
		Completed,
		[Display(Name = "发送失败")]
		Failed
	}
}
