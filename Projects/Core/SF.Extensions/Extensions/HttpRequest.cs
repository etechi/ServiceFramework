using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace SF
{
    public static class HttpRequestExtension
    {
        public static HttpResponseMessage CreateJSRedirectWithoutHistory(
            this HttpRequestMessage request,
            string uri
            )
        {
            uri = new Uri(request.RequestUri, uri).ToString();
            var ctn = new StringContent(
@"<html><body><script>
try{
    window.location.replace("""+uri+ @""");
}catch(e){
    window.location.href=""" + uri + @"""
}
</script></body></html>", 
                Encoding.UTF8, 
                "text/html"
                );
            
            var resp = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            resp.Content = ctn;
            return resp;
        }

        public static HttpResponseMessage CreateRedirect(
            this HttpRequestMessage request,
            string uri
            )
        {
            var resp = new HttpResponseMessage(System.Net.HttpStatusCode.Redirect);
            resp.Headers.Location = new Uri(request.RequestUri, uri);
            return resp;
        }
    }
}
