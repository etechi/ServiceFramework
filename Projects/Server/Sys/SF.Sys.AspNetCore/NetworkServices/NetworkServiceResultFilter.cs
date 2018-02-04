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

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;
using SF.Sys.NetworkService;
using System.Linq;
using System.Net.Http;
using SF.Sys.Auth;
using SF.Sys.Logging;
using System.Text;

namespace SF.Sys.AspNetCore.NetworkServices
{
	class NetworkServiceResultFilter : IResultFilter
	{
		ILogger Logger { get; }
		public NetworkServiceResultFilter(ILogService LogService)
		{
			this.Logger = LogService.GetLogger("api");
		}
		public void OnResultExecuted(ResultExecutedContext context)
		{
		}
		IActionResult MapHttpContent(HttpContent ctn)
		{
			if (ctn is System.Net.Http.StringContent sctn)
				return new ContentResult
				{
					Content = sctn.ReadAsStringAsync().Result,
					ContentType = sctn.Headers.ContentType.MediaType,
				};

			if(ctn is System.Net.Http.StreamContent ssctn)
				return new FileStreamResult(
					ssctn.ReadAsStreamAsync().Result,
					new MediaTypeHeaderValue(
						ssctn.Headers.ContentType.MediaType
						)
					{
						Charset = ctn.Headers.ContentType.CharSet
					}
					);

			if(ctn is HttpResponse.FileContent fctn)
				return new PhysicalFileResult(
					fctn.FilePath,
					new MediaTypeHeaderValue(
						fctn.Headers.ContentType.MediaType
						)
					{
						Charset = ctn.Headers.ContentType.CharSet
					});

			return new FileContentResult(
					ctn.ReadAsByteArrayAsync().Result,
					new MediaTypeHeaderValue(
						ctn.Headers.ContentType.MediaType
						)
					{
						Charset = ctn.Headers.ContentType.CharSet
					}
					);
		}
		IActionResult MapContent(IContent ctn)
		{
			if (ctn is IStringContent sctn)
				return new ContentResult
				{
					Content = sctn.Content,
					ContentType = sctn.ContentType
				};

			if (ctn is IStreamContent ssctn)
				return new FileStreamResult(
					ssctn.Stream,
					new MediaTypeHeaderValue(
						ssctn.ContentType
						)
					{
						Charset = (ctn.Encoding ?? System.Text.Encoding.UTF8).WebName
					}
					);

			if (ctn is IFileContent fctn)
				return new PhysicalFileResult(
					fctn.FilePath,
					new MediaTypeHeaderValue(
						fctn.ContentType
						)
					{
						Charset = (ctn.Encoding ?? System.Text.Encoding.UTF8).WebName
					});

			return new FileContentResult(
					ctn.GetByteArrayAsync().Result,
					new MediaTypeHeaderValue(
						ctn.ContentType
						)
					{
						Charset = (ctn.Encoding ?? System.Text.Encoding.UTF8).WebName
					}
					);
		}
		void ProcessHttpResponseMessage(ResultExecutingContext context,HttpResponseMessage res)
		{
			var cres = context.HttpContext.Response;
			cres.StatusCode = (int)res.StatusCode;
			foreach (var h in res.Headers)
				cres.Headers.Add(h.Key, h.Value.ToArray());
			if(res.Content!=null)
				context.Result=MapHttpContent(res.Content);
		}

		public void OnResultExecuting(ResultExecutingContext context)
		{
			string logMsg = null;			
			var or = context.Result as ObjectResult;
			if (or != null)
			{
				//if(or.Value is MvcObject)
				if (or.Value is HttpResponseMessage rm)
				{
					ProcessHttpResponseMessage(context, rm);
					logMsg = "HttpResponseMessage Object";
				}
				else if (or.Value is IContent ictn)
				{
					context.Result = MapContent(ictn);
					logMsg = "IContent Object";
				}
				else if (or.Value is HttpContent ctn)
				{
					context.Result = MapHttpContent(ctn);
					logMsg = "HttpContent Object";
				}
				else
					logMsg = "Json:" + Json.Stringify(or.Value);
			}
			var ctx = context.HttpContext;
			var req = ctx.Request;
			var reqBody = ctx.Items["sf-req-body"] as byte[];
			Logger.Info(
				"{0} {1} {2} 请求:{3} 应答:{4}",
				ctx.User.GetUserIdent(),
				req.Method,
				req.Uri(),
				reqBody==null?"":Encoding.UTF8.GetString(reqBody),
				logMsg
				);
		}
	}
}
