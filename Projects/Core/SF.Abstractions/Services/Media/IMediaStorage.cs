using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Services.Media
{
    public interface IMediaStorage
	{
		Task<IMediaMeta> ResolveAsync(string Id);
        Task<bool> RemoveAsync(string id);
		Task<IContent> GetContentAsync(IMediaMeta Media);
		Task<string> SaveAsync(IMediaMeta Meta, IContent Content);
	}
}
