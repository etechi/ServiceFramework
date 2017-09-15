using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF
{
	public interface IContent
	{
		Task<System.IO.Stream> OpenStreamAsync();
		Task<byte[]> GetByteArrayAsync();
		Task<string> GetStringAsync();
		string ContentType { get; }
		string FileName { get; }
		string Encoding { get; }
	}
    public interface IStreamContent : IContent
	{
        System.IO.Stream Stream { get; }
		
    }
    public interface IFileContent : IContent
	{
		string Path { get; }
	}
	public interface IByteArrayContent : IContent
	{
		byte[] Data { get; }
	}
	
}

