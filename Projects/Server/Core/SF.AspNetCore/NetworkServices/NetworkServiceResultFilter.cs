using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;
using SF.Core.NetworkService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;

namespace SF.AspNetCore.NetworkServices
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
