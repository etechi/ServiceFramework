using SF.Common.Notifications.Senders;
using SF.Sys.Auth;
using SF.Sys.Collections.Generic;
using SF.Sys.Threading;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SF.Externals.Aliyun
{
	public class AliyunException : ApplicationException
	{
		public AliyunException(string Code,string Message,string RequestId,string HostId) : base(Message)
		{
			this.Code = Code;
			this.RequestId = RequestId;
			this.HostId = HostId;
		}
		public string RequestId { get; set; }
		public string HostId { get; set; }
		public string Code { get; set; }
	}
	public class AliyunApiSetting
	{
		public string Version { get; set; }
		public string RegionId { get; set; }
	}
	
	public interface IAliyunInvoker
	{
		Task<string> Invoke(
			string Method,
			string BaseUri,
			(string Key, string Value)[] ApiSettings,
			params (string Key, string Value)[] Arguments

		);
	}
}
