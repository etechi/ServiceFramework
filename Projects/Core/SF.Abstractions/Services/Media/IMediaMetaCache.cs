using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Services.Media
{
	public interface IMediaMetaCache
	{
		IMediaMeta TryGet(string Id);
		IMediaMeta GetOrAdd(IMediaMeta Media);
        void Remove(string Id);
	}
}
