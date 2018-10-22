#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0

using SF.Sys.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SF.Sys.HttpClients
{

	public class HttpClient : IHttpClient
	{
		public ILogger Logger { get; }
		public HttpClient(ILogService LogService)
		{
			Logger = LogService?.GetLogger("HTTP");
		}
		
		public async Task<T> Request<T>(HttpRequestMessage Request,Func<HttpResponseMessage,Task<T>> GetResult)
		{
            var handler = new HttpClientHandler
            {
                AllowAutoRedirect=true,
                Proxy=null,
                // UseDefaultCredentials = false,
                //Proxy = new WebProxy("http://101.132.131.224:13002", false, new string[] { }),
                UseProxy = false
            };

            //var cli = new System.Net.Http.HttpClient(handler);
            var cli = new System.Net.Http.HttpClient(handler);

			string respData=null;
			string reqData=null;
			HttpStatusCode StatusCode = default;
			try
			{
				reqData = await GetReqData(Request);
				
				using (var resp = await cli.SendAsync(Request))
				{
					StatusCode = resp.StatusCode;
					respData = await GetRespData(resp);

					var re = await GetResult(resp);

					Log(Request,respData,reqData,StatusCode,null);
					return re;
				}
			}
			catch (Exception ex)
			{
				Log(Request, respData, reqData, StatusCode, ex);
				throw;
			}
		}

		private void Log(HttpRequestMessage Request, string respData, string reqData, HttpStatusCode StatusCode, Exception ex)
		{
			if (Logger == null)
				return;
			if(ex==null)
				Logger.Info(
					"{0} {1} 请求:{2} 应答:{3} {4}",
					Request.Method,
					Request.RequestUri,
					reqData,
					StatusCode,
					respData
					);
			else
				Logger.Error(
					ex,
					"{0} {1} 请求:{2} 应答:{3} {4}",
					Request.Method,
					Request.RequestUri,
					reqData,
					StatusCode,
					respData
					);
		}

		private static async Task<string> GetRespData(HttpResponseMessage resp)
		{
			if (resp.Content != null)
			{
				var respHeader = resp.Content.Headers;

				if (respHeader.ContentLength < 1000 * 100 &&
					respHeader.ContentType.MediaType == "application/json")
				{
					var respData=await resp.Content.ReadAsStringAsync();
					resp.Content = new StringContent(respData);
					return respData;
				}
				else
					return $"{respHeader.ContentType.MediaType} {respHeader.ContentLength}";
			}
			else
				return "Empty";
		}

		private static async Task<string> GetReqData(HttpRequestMessage Request)
		{
			if (Request.Content != null)
			{
				var reqHeaders = Request.Content.Headers;
				if (reqHeaders.ContentLength < 1024 * 100 &&
					(reqHeaders.ContentType.MediaType == "application/json" ||
					reqHeaders.ContentType.MediaType == "application/x-www-form-urlencoded")
					)
					return await Request.Content.ReadAsStringAsync();
				else
					return $"{reqHeaders.ContentType.MediaType} {reqHeaders.ContentLength}";
			}
			return "Empty";
		}
	}
	
}
