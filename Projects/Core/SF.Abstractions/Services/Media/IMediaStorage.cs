using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Services.Media
{
	[Metadata.Comment("媒体数据存储")]
	public interface IMediaStorage
	{
		Task<IMediaMeta> ResolveAsync(string IDPrefix, string Id);
        Task<bool> RemoveAsync(string id);
		Task<IContent> GetContentAsync(IMediaMeta Media);
		Task<string> SaveAsync(IMediaMeta Meta, IContent Content);
	}
}
