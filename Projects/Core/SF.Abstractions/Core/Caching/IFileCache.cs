using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Core.Caching
{
	public class FileCacheContent
	{
		public string FileExtension { get; set; }
		public byte[] Content { get; set; }
	}
	
	public delegate Task<FileCacheContent> FileContentGenerator();
	public interface IFileCache
	{
		Task<string> Cache(
		   string FileName,
		   FileContentGenerator ContentGenerator,
		   string FilePath = null
		   );
	}
}
