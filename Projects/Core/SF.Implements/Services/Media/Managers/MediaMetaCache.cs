using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProtocol.Biz.Media
{
	public class MediaMetaCache : IMediaMetaCache
	{
		public Caching.ICache Cache { get; }
		public static readonly string CacheKeyPrefix="SP.Biz.Media.Meta.";
		public MediaMetaCache(Caching.ICache cache)
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
