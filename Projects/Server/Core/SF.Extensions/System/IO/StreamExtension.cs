using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace System.IO
{
	public static class StreamExtension
	{
		public static byte[] ReadToEnd(this Stream stream)
		{
			var ms = new MemoryStream();
			stream.CopyTo(ms);
			return ms.ToArray();
		}
		public static async Task<byte[]> ReadToEndAsync(this Stream stream)
		{
			var ms = new MemoryStream();
			await stream.CopyToAsync(ms);
			return ms.ToArray();
		}
	}
}
