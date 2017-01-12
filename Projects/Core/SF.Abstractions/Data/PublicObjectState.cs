using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Data
{
	public enum LogicObjectState : byte
	{
		[Display(Name ="有效")]
		Enabled=0,
		[Display(Name = "无效")]
		Disabled =1,
		[Display(Name = "已删除")]
		Deleted =2
	}
}
