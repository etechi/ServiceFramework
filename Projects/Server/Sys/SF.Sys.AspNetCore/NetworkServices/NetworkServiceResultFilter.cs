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

namespace SF.Sys.AspNetCore.NetworkServices
{
	class NetworkServiceResultFilter : IResultFilter
	{
		public void OnResultExecuted(ResultExecutedContext context)
		{
		}
		IActionResult ProcessHttpContent(HttpContent ctn)
		{
			var sctn = ctn as StringContent;
			if (sctn != null)
				return new ContentResult
				{
					Content = sctn.ReadAsStringAsync().Result,
					ContentType = sctn.Headers.ContentType.MediaType,
				};

			var ssctn = ctn as System.Net.Http.StreamContent;
			if (ssctn != null)
				return new FileStreamResult(
					sctn.ReadAsStreamAsync().Result,
					new MediaTypeHeaderValue(
						sctn.Headers.ContentType.MediaType
						)
					{
						Charset = ctn.Headers.ContentType.CharSet
					}
					);
			var fctn = ctn as HttpResponse.FileContent;
			if (fctn != null)
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
						sctn.Headers.ContentType.MediaType
						)
					{
						Charset = ctn.Headers.ContentType.CharSet
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
				context.Result=ProcessHttpContent(res.Content);
		}
		public void OnResultExecuting(ResultExecutingContext context)
		{
			var or = context.Result as ObjectResult;
			if (or != null)
			{
				var res = or.Value as HttpResponseMessage;
				if (res != null)
					ProcessHttpResponseMessage(context, res);
				else
				{
					var ctn = or.Value as HttpContent;
					if (ctn != null)
						context.Result = ProcessHttpContent(ctn);
				}
			}
		}
	}
}
