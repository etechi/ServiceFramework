#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0

using SF.Core.ServiceManagement;
using SF.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Services.Media
{

	public class MediaManagerSetting
	{

		[Comment("媒体元信息缓存")]
		[Optional]
		public IMediaMetaCache MetaCache { get; set; }
	}
	[Comment("媒体管理器")]
	public class MediaManager : IMediaManager
	{
		public IMediaMetaCache MetaCache { get; }
		public NamedServiceResolver<IMediaStorage> MediaStorageResolver { get; }
		public MediaManager(IMediaMetaCache MetaCache,NamedServiceResolver<IMediaStorage> MediaStorageResolver)
		{
			this.MetaCache= MetaCache;
			this.MediaStorageResolver = MediaStorageResolver;
		}
		IMediaStorage GetStorage(string Type,string Id)
		{
			var stg = MediaStorageResolver(Type);
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

			m = await stg.ResolveAsync(type, Id.Substring(i+1));
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
        public Task<IContent> GetContentAsync(IMediaMeta Meta)
		{
			Ensure.NotNull(Meta, nameof(Meta));
			var stg = GetStorage(Meta.Type, Meta.Id);
			return stg.GetContentAsync(Meta);
		}

		public async Task<string> SaveAsync(IMediaMeta Meta, IContent Content)
		{
			Ensure.NotNull(Meta,nameof(Meta));
			Ensure.NotNull(Content, nameof(Content));
			var stg = GetStorage(Meta.Type, Meta.Id);
			var re=await stg.SaveAsync( Meta, Content);
			return Meta.Type + "-" + re;
		}
	}
}
