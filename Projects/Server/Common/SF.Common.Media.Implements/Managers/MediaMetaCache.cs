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

using SF.Sys.Caching;
using System;

namespace SF.Common.Media
{
	/// <summary>
	/// 媒体元信息缓存
	/// </summary>
	public class MediaMetaCache : IMediaMetaCache
	{
		public ILocalCache<IMediaMeta> Cache { get; }
		public MediaMetaCache(ILocalCache<IMediaMeta> cache)
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
