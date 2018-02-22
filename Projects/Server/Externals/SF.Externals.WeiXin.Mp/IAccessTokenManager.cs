using SF.Sys;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SF.Externals.WeiXin.Mp
{
	public class WeiXinException : ExternalServiceException
	{
		public int Code { get; }
		public WeiXinException(int Code,string Message) : base(Message)
		{
			this.Code = Code;
		}
	}
	public interface IAccessTokenManager
    {
        Task<string> GetAccessToken();
        Task<string> GetJsApiTicket();
		Task<string> RequestString(Uri uri, HttpContent Content);
	}
}
