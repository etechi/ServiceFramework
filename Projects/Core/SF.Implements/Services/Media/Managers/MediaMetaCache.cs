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
		public SF.Core.Caching.ILocalCache Cache { get; }
		public static readonly string CacheKeyPrefix="SP.Biz.Media.Meta.";
		public MediaMetaCache(SF.Core.Caching.ILocalCache cache)
		{
			Cache = cache;
		}
		string CacheKey(string Id)
		{
			return CacheKeyPrefix + Id;
		}
        public void Remove(string Id)
        {
            Cache.Remove(Id);
        }
		public IMediaMeta TryGet(string Id)
		{
			return Cache.Get(CacheKey(Id)) as IMediaMeta;
		}
		public IMediaMeta GetOrAdd(IMediaMeta Media)
		{
			return Cache.AddOrGetExisting(CacheKey(Media.Id), Media, TimeSpan.FromHours(1)) as IMediaMeta ?? Media;
		}
	}
}
