using SF.Auth;
using SF.Metadata;
using SF.Core.NetworkService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SF.Services.Settings
{
	[Comment("系统设置")]
	public class SystemSetting
	{
		[Comment(Name = "系统名称", Description = "系统名称")]
		public string SystemName { get; set; } = System.IO.Path.GetDirectoryName( AppDomain.CurrentDomain.BaseDirectory);
	}
}
