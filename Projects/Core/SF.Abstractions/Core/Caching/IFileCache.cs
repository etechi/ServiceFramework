using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Core.Caching
{
	public interface IFileCache
	{
		Task<string> Cache(
		   string FileName,
		   System.Threading.SemaphoreSlim Locker,
		   Func<Task<KeyValuePair<string, byte[]>>> Loader,
		   string FilePath = null
		   );
	}
}
