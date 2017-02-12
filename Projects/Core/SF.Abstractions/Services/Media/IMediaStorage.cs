using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Services.Media
{
    public interface IMediaStorage
	{
		Task<IMediaMeta> ResolveAsync(string RootPath, string IDPrefix, string Id);
        Task<bool> RemoveAsync(string RootPath, string id);
		Task<IContent> GetContentAsync(IMediaMeta Media);
		Task<string> SaveAsync(string RootPath, IMediaMeta Meta, IContent Content);
	}
}
