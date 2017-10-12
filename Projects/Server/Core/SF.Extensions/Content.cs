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

namespace SF
{
	public abstract class BaseContent : IContent
	{
		public string ContentType { get; set; }

		public string FileName { get; set; }
		public string Encoding { get; set; }

		public virtual async Task<System.IO.Stream> OpenStreamAsync()
		{
			return new MemoryStream(await GetByteArrayAsync());
		}
		public virtual async Task<byte[]> GetByteArrayAsync()
		{
			using (var s = await OpenStreamAsync())
			{
				return await s.ReadToEndAsync();
			}
		}

		public virtual async Task<string> GetStringAsync()
		{
			return (Encoding == null ? System.Text.Encoding.UTF8 : System.Text.Encoding.GetEncoding(Encoding)).GetString(
				await GetByteArrayAsync()
				);
		}
	}

	public class FileContent : BaseContent,IFileContent
	{
		public string Path { get; set; }

		public override Task<System.IO.Stream> OpenStreamAsync()
		{
			return Task.FromResult(
				(System.IO.Stream)new System.IO.FileStream(
					Path, 
					System.IO.FileMode.Open, 
					System.IO.FileAccess.Read, 
					System.IO.FileShare.Read
				)
			);
		}
	}
    public class StreamContent : BaseContent, IStreamContent
    {
        public System.IO.Stream Stream { get; set; }
        public override Task<System.IO.Stream> OpenStreamAsync()
        {
            return Task.FromResult(Stream);
        }
        public override async Task<byte[]> GetByteArrayAsync()
        {
            return await Stream.ReadToEndAsync();
        }
    }
    public class ByteArrayContent : BaseContent, IByteArrayContent
	{
		public byte[] Data { get; set; }
		
		public override Task<byte[]> GetByteArrayAsync()
		{
			return Task.FromResult(Data);
		}
	}
}

