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

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.HttpOverrides;
using System;
using SF.Sys.Logging;
using SF.Sys.Auth;
using System.Text;
using System.Threading.Tasks;

namespace SF.Sys.AspNetCore
{
	
	public static class RequestLoggingExtension
	{
		static void Log(
			Microsoft.AspNetCore.Http.HttpContext context, 
			byte[] reqData, 
			long time,
			Exception error
			)
		{
			var logService = (ILogService)context
				.RequestServices
				.GetService(typeof(ILogService));
			if (logService == null)
				return;

			var logger = logService.GetLogger("api");
			var req = context.Request;
			var respMessage =(string) context.Items[HttpContextResponseItemKey.Result];
			if (error == null)
				error = (Exception)context.Items[HttpContextResponseItemKey.Error];
			if (error == null)
			{
				logger.Info(
					"User:{0} {1} {2} {3}ms REQ:{4} RESP:{5}",
					context.User.GetUserIdent(),
					req.Method, 
					req.Uri(),
					time,
					reqData == null ? "" : Encoding.UTF8.GetString(reqData),
					respMessage ?? ""
					);
			}
			else
				logger.Error(
					error,
					"User:{0} {1} {2} {3}ms REQ:{4} RESP:{5} ERR:{6}",
					context.User.GetUserIdent(),
					req.Method,
					req.Uri(),
					time,
					reqData == null ? "" : Encoding.UTF8.GetString(reqData),
					respMessage ?? "",
					error.Message
					);
		}
		static async Task<byte[]> GetRequestData(Microsoft.AspNetCore.Http.HttpContext context)
		{
			var req = context.Request;
			if (req.ContentType!=null &&
				(req.ContentType.StartsWith("application/json") ||
				req.ContentType.StartsWith("text/json") ||
                req.ContentType.StartsWith("text/plain") ||
                req.ContentType.StartsWith("application/x-www-form-urlencoded")
				)
				&& req.ContentLength.HasValue &&
				req.ContentLength.Value < 1024 * 1024)
			{
				var buf = new byte[req.ContentLength.Value];
				for (var i = 0; i < buf.Length;)
				{
					var re = await req.Body.ReadAsync(buf, i, buf.Length - i);
					if (re == 0)
						throw new ArgumentException("读取不到足够数据");
					i += re;
				}
				req.Body = new System.IO.MemoryStream(buf);
				return buf;
			}
            var ctn = $"{req.ContentType} {req.ContentLength}bytes";
            if (req.ContentType?.StartsWith("multipart/form-data;") ??false)
                for (var i = 0; i < req.Form.Files.Count; i++)
                {
                    var f = req.Form.Files[i];
                    ctn += $" file{i}(key={f.Name} file={f.FileName} mime={f.ContentType} size={f.Length})";
                }
			return Encoding.UTF8.GetBytes(ctn);
		}
		public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app) {
			return app.Use(async (context,next) =>
			{
				byte[] reqData = null;
				long time = 0;
				try
				{
					reqData = await GetRequestData(context);
					var sw = System.Diagnostics.Stopwatch.StartNew();
					try
					{
						await next();
					}
					finally
					{
						sw.Stop();
						time = sw.ElapsedMilliseconds;
					}
					Log(context, reqData, time,null);
				}
				catch (PublicException err)
				{
					context.Response.StatusCode = 500;
					context.Response.ContentType = "text/json; charset=utf8";
#if DEBUG
					var buf = Json.Stringify(err).UTF8Bytes();
#else
					var buf = Json.Stringify(new {
						Message=err.Message,
						ClassName=err.GetType().FullName
					}).UTF8Bytes();

#endif
					context.Response.ContentLength = buf.Length;
					await context.Response.Body.WriteAsync(buf, 0, buf.Length);
					Log(context, reqData, time, err);
				}
				catch(Exception err)
				{
					Log(context, reqData, time,err);
					throw;
				}
			});
		}
	}
}
