using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceProtocol.DI;

namespace ServiceProtocol.Biz.Media
{
	public class MediaManager : IMediaManager
	{
		public IMediaMetaCache MetaCache { get; }
		public IMediaStorage MediaStorage { get; }
		public DI.IDIProviderResolver<IMediaStorage> ProviderResolver { get; }
		public MediaManager(IMediaMetaCache MetaCache, DI.IDIProviderResolver<IMediaStorage> ProviderResolver)
		{
			this.MetaCache = MetaCache;
			this.MediaStorage = MediaStorage;
			this.ProviderResolver = ProviderResolver;
		}
		IMediaStorage GetStorage(string Type,string Id)
		{
			var stg = ProviderResolver.Resolve(Type);
			if (stg == null) throw new ArgumentException("不支持此媒体存储类型:" + Type + "," + Id);
			return stg;
		}
		public async Task<IMediaMeta> ResolveAsync(string Id)
		{
			var m = MetaCache.TryGet(Id);
			if (m != null) return m;

			var i = Id.IndexOf('-');
			if (i == -1) return null;
			var type = Id.Substring(0, i);
			var stg = GetStorage(type, Id);

			m = await stg.ResolveAsync(Id.Substring(i+1));
			if (m == null)
				return null;
			if (type + "-" + m.Id != Id)
				throw new InvalidOperationException();
			return MetaCache.GetOrAdd(m);
		}
        public async Task RemoveAsync(string Id)
        {
            MetaCache.Remove(Id);

            var i = Id.IndexOf('-');
            if (i == -1)
                return;
            var type = Id.Substring(0, i);
            var stg = GetStorage(type, Id);
            await stg.RemoveAsync(Id.Substring(i + 1));
        }
        public Task<IMediaContent> GetContentAsync(IMediaMeta Meta)
		{
			Ensure.NotNull(Meta, nameof(Meta));
			var stg = GetStorage(Meta.Type, Meta.Id);
			return stg.GetContentAsync(Meta);
		}

		public async Task<string> SaveAsync(IMediaMeta Meta, IMediaContent Content)
		{
			Ensure.NotNull(Meta,nameof(Meta));
			Ensure.NotNull(Content, nameof(Content));
			var stg = GetStorage(Meta.Type, Meta.Id);
			var re=await stg.SaveAsync(Meta, Content);
			return Meta.Type + "-" + re;
		}
	}
}
