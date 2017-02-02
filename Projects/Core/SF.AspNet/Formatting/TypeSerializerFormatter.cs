using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Dependencies;
using System.Web;
using System.Net.Http.Formatting;
using System.IO;
using System.Net.Http;
using System.Net;
using SF.Core.Serialization;

namespace SF.AspNet.Formatting
{

	public class JsonSerializerFormatter : MediaTypeFormatter
	{
		IJsonSerializer Serializer { get; }
		public JsonSerializerFormatter(IJsonSerializer Serializer, string mime = null)
		{
			SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue(mime ?? "application/json"));
			this.Serializer = Serializer;
		}
		public override bool CanReadType(Type type)
		{
			return true;
		}
		public override bool CanWriteType(Type type)
		{
			return true;
		}
		Encoding GetEncoding(HttpContent content)
		{
			var encoding = content.Headers.ContentEncoding.FirstOrDefault();
			return encoding == null ? Encoding.UTF8 : Encoding.GetEncoding(encoding);
		}
		public override async Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
		{
			var buf = await readStream.ReadToEndAsync();
			var str = GetEncoding(content).GetString(buf);
			var re =Serializer.Deserialize(str, type);
			return re;

		}
		public override async Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content, TransportContext transportContext)
		{
			var ms = new MemoryStream();
			var str=Serializer.Serialize(value);
			var buf = GetEncoding(content).GetBytes(str);
			await writeStream.WriteAsync(buf, 0, buf.Length);
		}

	}

}
