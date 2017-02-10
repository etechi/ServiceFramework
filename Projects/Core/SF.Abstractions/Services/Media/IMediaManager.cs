using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Services.Media
{
	public interface IMediaManager
	{
		Task<IMediaMeta> ResolveAsync(string Id);
		Task<IContent> GetContentAsync(IMediaMeta Meta);
		Task<string> SaveAsync(IMediaMeta Meta, IContent Content);
        Task RemoveAsync(string Id);
    }
}
