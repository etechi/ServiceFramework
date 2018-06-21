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
using System.Text;
using System.Threading.Tasks;
namespace SF.Sys.IO
{
	public static class StreamExtension
	{
		public static string ReadString(this Stream stream,Encoding Encoding=null, bool detectEncodingFromByteOrderMarks=true, bool Dispose = false)
		{
			try
			{
				using (var sr = new StreamReader(stream, Encoding ?? Encoding.UTF8, detectEncodingFromByteOrderMarks))
				{
					return sr.ReadToEnd();
				}
			}
			finally
			{
				if (Dispose)
					stream.Dispose();
			}
		}
		public static async Task<string> ReadStringAsync(this Stream stream, Encoding Encoding = null, bool detectEncodingFromByteOrderMarks = true, bool Dispose = false)
		{
			try
			{
				using (var sr = new StreamReader(stream, Encoding ?? Encoding.UTF8, detectEncodingFromByteOrderMarks))
				{
					return await sr.ReadToEndAsync();
				}
			}
			finally
			{
				if (Dispose)
					stream.Dispose();
			}
		}
		public static byte[] ReadToEnd(this Stream stream,bool Dispose=false)
		{
			try
			{
				var ms = new MemoryStream();
				stream.CopyTo(ms);
				return ms.ToArray();
			}
			finally
			{
				if (Dispose)
					stream.Dispose();
			}
		}
		public static async Task<byte[]> ReadToEndAsync(this Stream stream,bool Dispose=false)
		{
			try
			{
				var ms = new MemoryStream();
				await stream.CopyToAsync(ms);
				return ms.ToArray();
			}
			finally
			{
				if (Dispose)
					stream.Dispose();

			}
		}
		
	}
}
