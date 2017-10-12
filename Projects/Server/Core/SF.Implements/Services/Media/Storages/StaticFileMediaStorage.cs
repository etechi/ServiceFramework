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

using SF.Core.Hosting;
using SF.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Services.Media.Storages
{
	[Comment("本地只读媒体文件存储")]
	public class StaticFileMediaStorage : IMediaStorage
	{
		public KB.Mime.IMimeResolver MimeResolver { get; }
		public string RootPath { get;  }
		public IFilePathResolver PathResolver { get; }

		public StaticFileMediaStorage(
			KB.Mime.IMimeResolver MimeResolver,
			[Comment("文件根目录")]
			string RootPath,
			[Metadata.Comment("路径解析器")]
			IFilePathResolver PathResolver
			)
		{
			this.RootPath = RootPath;
			this.MimeResolver = MimeResolver;
			this.PathResolver = PathResolver;
		}
		public Task<bool> RemoveAsync(string Id)
        {
            throw new NotSupportedException();
        }
		public Task<IMediaMeta> ResolveAsync(string IDPrefix,string Id)
		{
			if (Id[0]=='.' || Id[0]=='/' || Id[0]=='\\' || Id.Contains(':'))
				throw new NotSupportedException();
			var lid = Id.LastIndexOf('-');
			if (lid == -1)
				return null;
			var file= Id.Substring(0, lid).Replace('-',System.IO.Path.DirectorySeparatorChar) + "." + Id.Substring(lid + 1);

			var path = PathResolver.Resolve(RootPath, file);
			if (!System.IO.File.Exists(path))
				return Task.FromResult((IMediaMeta)null);
			var mime = MimeResolver.FileExtensionToMime(System.IO.Path.GetExtension(path));
			return Task.FromResult(
				(IMediaMeta)new FileMediaMeta
				{
					Id = Id,
					Type = IDPrefix,
					Mime = mime,
					Name = System.IO.Path.GetFileNameWithoutExtension(path),
					Path = path
				});
		}

		public Task<IContent> GetContentAsync(IMediaMeta Media)
		{
			var fm = Media as FileMediaMeta;
			if (fm == null)
				throw new NotSupportedException();
			return Task.FromResult(
				(IContent)new FileContent {
					Path = fm.Path,
					ContentType=Media.Mime,
					FileName=Media.Name
				}
				);
		}

		public Task<string> SaveAsync(IMediaMeta media, IContent Content)
		{
			throw new NotSupportedException();
		}
	}
}
