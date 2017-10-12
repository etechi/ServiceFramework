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
