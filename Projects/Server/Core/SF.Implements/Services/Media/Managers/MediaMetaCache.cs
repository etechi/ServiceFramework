using SF.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Services.Media
{
	[Comment("媒体元信息缓存")]
	public class MediaMetaCache : IMediaMetaCache
	{
		public SF.Core.Caching.ILocalCache<IMediaMeta> Cache { get; }
		public MediaMetaCache(SF.Core.Caching.ILocalCache<IMediaMeta> cache)
		{
			Cache = cache;
		}
		
        public void Remove(string Id)
        {
            Cache.Remove(Id);
        }
		public IMediaMeta TryGet(string Id)
		{
			return Cache.Get(Id) as IMediaMeta;
		}
		public IMediaMeta GetOrAdd(IMediaMeta Media)
		{
			return Cache.AddOrGetExisting(Media.Id, Media, TimeSpan.FromHours(1)) as IMediaMeta ?? Media;
		}
	}
}
