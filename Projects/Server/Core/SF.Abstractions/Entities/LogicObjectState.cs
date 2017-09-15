using SF.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Entities
{
	public enum EntityLogicState : byte
	{
		[Comment(Name ="有效")]
		Enabled=0,
		[Comment(Name = "无效")]
		Disabled =1,
		[Comment(Name = "已删除")]
		Deleted =2
	}
}
