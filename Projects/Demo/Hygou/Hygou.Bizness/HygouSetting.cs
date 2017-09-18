using SF.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Hygou
{
    public class HygouSetting
    {
		[Required]
		[Comment(GroupName = "帮助中心", Name = "PC站点默认帮助文档", Description = "默认帮助中心文档")]
		public long PCHelpCenterDefaultDocId { get; set; }
	}


}
