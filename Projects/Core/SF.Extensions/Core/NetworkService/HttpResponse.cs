using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
namespace SF
{
	public static class HttpResponse
	{
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
				Content = new StringContent(Text, Encoding ?? Encoding.UTF8, mediaType)
			};

		public static HttpResponseMessage Json<T>(T Object, Encoding Encoding = null, System.Net.HttpStatusCode Status = System.Net.HttpStatusCode.OK) =>
			Text(SF.Core.Json.Stringify(Object), "application/json", Encoding, Status);

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
