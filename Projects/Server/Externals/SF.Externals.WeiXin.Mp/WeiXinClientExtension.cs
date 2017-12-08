using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SF.Externals.WeiXin.Mp
{
    public static class WeiXinClientExtension
    {
        public static Task<string> Json<T>(this IWeiXinClient client,string uri, T data)
        {
            return client.RequestString(
                uri, 
                new StringContent(
                    SF.Sys.Json.Stringify(data), 
                    Encoding.UTF8, 
                    "application/json"
                    )
                );
        }
        
    }
}
