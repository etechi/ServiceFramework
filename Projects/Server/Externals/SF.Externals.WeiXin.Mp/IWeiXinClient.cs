using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SF.Externals.WeiXin.Mp
{
  
    public class JsApiSignatureResult
    {
        public string appId { get; set; }
	    public string timestamp { get; set; }
	    public string nonceStr { get; set; }
	    public string signature { get; set; }
    }
    public interface IWeiXinClient
    {
        Task<string> RequestString(string uri, HttpContent Content);
        Task<JsApiSignatureResult> JsApiSignature(string uri);
        Task<string> GetMediaId(string serverId);
    }
}
