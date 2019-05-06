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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using SF.Sys;
namespace SF.Sys.NetworkService
{
	public static class HttpResponse
	{
		public static HttpResponseMessage NotModified=>
			new HttpResponseMessage(System.Net.HttpStatusCode.NotModified);
		public static HttpResponseMessage NotFound=>
			new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
		public static HttpResponseMessage Redirect(Uri Uri)
		{
			var m = new HttpResponseMessage(System.Net.HttpStatusCode.Redirect);
			m.Headers.Location = Uri;
			return m;
		}
        public static HttpResponseMessage JSRedirectWithoutHistory(Uri uri)
        {
            var ctn = new System.Net.Http.StringContent(
@"<html><body><script>
try{
    window.location.replace(""" + uri + @""");
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

        public static HttpResponseMessage ByteArray(byte[] Bytes, string mediaType = "application/octet", System.Net.HttpStatusCode Status = System.Net.HttpStatusCode.OK)
		{
			var ctn = new System.Net.Http.ByteArrayContent(Bytes);
			ctn.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(mediaType);
			return new HttpResponseMessage(Status)
			{
				Content = ctn
			};
		}
		public static HttpResponseMessage Text(string Text, string mediaType = "text/plain", Encoding Encoding = null, System.Net.HttpStatusCode Status = System.Net.HttpStatusCode.OK) =>
			new HttpResponseMessage(Status)
			{
				Content = new System.Net.Http.StringContent(Text, Encoding ?? Encoding.UTF8, mediaType)
			};

		public static HttpResponseMessage Json<T>(T Object, Encoding Encoding = null, System.Net.HttpStatusCode Status = System.Net.HttpStatusCode.OK) =>
			Text(SF.Sys.Json.Stringify(Object), "application/json", Encoding, Status);

		public class FileContent : HttpContent
		{
			public string FilePath { get; }

			public FileContent(string filePath, string contentType)
			{
				if (filePath == null) throw new ArgumentNullException("filePath");

				this.FilePath = filePath;
				this.Headers.ContentType = new MediaTypeHeaderValue(contentType);
			}

			protected override bool TryComputeLength(out long length)
			{
				var fi = new System.IO.FileInfo(FilePath);
				length = fi.Length;
				return true;
			}

			protected override async Task SerializeToStreamAsync(System.IO.Stream stream, TransportContext context)
			{
				using (var fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
					await fs.CopyToAsync(stream);
			}
		}
		public static HttpResponseMessage File(string FileName, string mediaType = "application/octet", System.Net.HttpStatusCode Status = System.Net.HttpStatusCode.OK)
		{
			var ctn = new FileContent(FileName, mediaType);
			return new HttpResponseMessage(Status)
			{
				Content = ctn
			};
		}
	}
}
