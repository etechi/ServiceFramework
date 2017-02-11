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

