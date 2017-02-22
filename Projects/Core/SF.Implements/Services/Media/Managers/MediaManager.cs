using SF.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Services.Media
{
	public class MediaStorageSetting
	{
		[Key]
		[Comment("媒体标识类型")]
		public string Type { get; set; }
		[Comment("媒体存储器")]
		public IMediaStorage Storage { get; set; }
	}
	public class MediaManagerSetting
	{
		[Comment("存储类型")]
		[TableRows]
		public Dictionary<string, MediaStorageSetting> Types { get; set; }
		public IMediaMetaCache MetaCache { get; set; }
	}
	[Comment("媒体管理器")]
	public class MediaManager : IMediaManager
	{
		public MediaManagerSetting Setting { get; }
		public MediaManager(MediaManagerSetting Setting)
		{
			this.Setting = Setting;
		}
		MediaStorageSetting GetStorage(string Type,string Id)
		{
			var stg = Setting.Types.Get(Type);
			if (stg == null) throw new ArgumentException("不支持此媒体存储类型:" + Type + "," + Id);
			return stg;
		}
		public async Task<IMediaMeta> ResolveAsync(string Id)
		{
			var m = Setting.MetaCache.TryGet(Id);
			if (m != null) return m;

			var i = Id.IndexOf('-');
			if (i == -1) return null;
			var type = Id.Substring(0, i);
			var stg = GetStorage(type, Id);

			m = await stg.Storage.ResolveAsync(stg.Type, Id.Substring(i+1));
			if (m == null)
				return null;
			if (type + "-" + m.Id != Id)
				throw new InvalidOperationException();
			return Setting.MetaCache.GetOrAdd(m);
		}
        public async Task RemoveAsync(string Id)
        {
            Setting.MetaCache.Remove(Id);

            var i = Id.IndexOf('-');
            if (i == -1)
                return;
            var type = Id.Substring(0, i);
            var stg = GetStorage(type, Id);
            await stg.Storage.RemoveAsync(Id.Substring(i + 1));
        }
        public Task<IContent> GetContentAsync(IMediaMeta Meta)
		{
			Ensure.NotNull(Meta, nameof(Meta));
			var stg = GetStorage(Meta.Type, Meta.Id);
			return stg.Storage.GetContentAsync(Meta);
		}

		public async Task<string> SaveAsync(IMediaMeta Meta, IContent Content)
		{
			Ensure.NotNull(Meta,nameof(Meta));
			Ensure.NotNull(Content, nameof(Content));
			var stg = GetStorage(Meta.Type, Meta.Id);
			var re=await stg.Storage.SaveAsync( Meta, Content);
			return Meta.Type + "-" + re;
		}
	}
}
