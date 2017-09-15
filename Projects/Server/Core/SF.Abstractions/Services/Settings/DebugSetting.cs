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
	public class DebugSetting
	{
		[Comment(Name = "调试模式", Description = "调试模式")]
		public bool DebugMode { get; set; }

		[Comment(Name = "调试用户ID", Description = "调试用户ID")]
		public int DebugUserId { get; set; }

	}
}
