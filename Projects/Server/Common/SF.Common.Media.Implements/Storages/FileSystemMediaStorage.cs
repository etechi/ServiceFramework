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

using SF.Sys;
using SF.Sys.Hosting;
using SF.Sys.Mime;
using SF.Sys.NetworkService;
using System;
using System.Threading.Tasks;

namespace SF.Common.Media.Storages
{
	class FileMediaMeta : IMediaMeta
	{
		public string Path { get; set; }
		public int Height{get;set;}
		public string Id{get;set;}
		public int Length{get;set;}
		public string Mime{get;set;}
		public string Name{get;set;}
		public string Type{get;set;}
		public int Width{get;set;}
	}
	/// <summary>
	/// 本地媒体文件存储
	/// </summary>
	public class FileSystemMediaStorage : IMediaStorage
	{
		public IMimeResolver MimeResolver { get; }
		public string RootPath { get; }
		public IFilePathResolver PathResolver { get; }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="MimeResolver"></param>
		/// <param name="RootPath">文件根目录</param>
		/// <param name="PathResolver">路径解析器</param>
		public FileSystemMediaStorage(
			IMimeResolver MimeResolver,
			string RootPath,
			IFilePathResolver PathResolver
			)
		{
			this.MimeResolver = MimeResolver;
			this.RootPath = RootPath;
			this.PathResolver = PathResolver;
		}
		
		string CalcGroup(Guid guid)
		{
			return (((uint)guid.GetHashCode()) % 97).ToString();

		}

        string FindFile(string Id)
        {
            Ensure.Equals(Id.Length, 8 + 32);
            var date = Id.Substring(0, 8);
            var gid = Id.Substring(8);
            var guid = Guid.ParseExact(gid, "N");
            var group = CalcGroup(guid);
            var path = PathResolver.Resolve(RootPath, date, group);
            if (!System.IO.Directory.Exists(path))
                return null;

            var fs = System.IO.Directory.GetFiles(path, $"{gid}*", System.IO.SearchOption.TopDirectoryOnly);

            if (fs.Length == 0)
                return null;
            return fs[0];
        }
        public Task<bool> RemoveAsync( string Id)
        {
            var file = FindFile(Id);
            if (file == null)
                return Task.FromResult(false);

            System.IO.File.Delete(file);
            return Task.FromResult(true);
        }
		public Task<IMediaMeta> ResolveAsync(string IDPrefix, string Id)
		{
            var file = FindFile(Id);
            if (file == null)
                return Task.FromResult<IMediaMeta>(null);
			
			var name = System.IO.Path.GetFileNameWithoutExtension(file).Substring(32);
			var ext = System.IO.Path.GetExtension(file);
			var mime = MimeResolver.FileExtensionToMime(ext);
			var li = name.LastIndexOf('-');
			if (li == -1)
				throw new ArgumentException("资源标示格式错误：" + Id);
			var ps = name.Substring(0,li).Split('-');
			if (ps.Length != 3)
				throw new ArgumentException("资源标示格式错误：" + Id);
			int width, height, length;
			if (!int.TryParse(ps[0], out width) || 
				!int.TryParse(ps[1], out height) ||
				!int.TryParse(ps[2], out length))
				throw new ArgumentException("资源标示格式错误：" + Id);

			var fn = name.Substring(li + 1);

			return Task.FromResult(
				(IMediaMeta)new FileMediaMeta
				{
					Id = Id,
					Type = IDPrefix,
					Height = height,
					Length = length,
					Width = width,
					Mime = mime,
					Name = fn,
					Path = file
				});
		}

		public Task<IContent> GetContentAsync(IMediaMeta Media)
		{
			var fm = Media as FileMediaMeta;
			if (fm == null)
				throw new NotSupportedException();
			return Task.FromResult(
				(IContent)new FileContent {
					FilePath = fm.Path ,
					ContentType = Media.Mime,
					FileName = Media.Name
				}
				);
		}

		public async Task<string> SaveAsync(IMediaMeta media, IContent Content)
		{
			if (Content == null)
				throw new ArgumentException();
			else  if(!(Content is IContent) )
				throw new ArgumentException();

			var guid = Guid.NewGuid();
			var id = guid.ToString("N").ToLower(); //{B2A6 FC3D-9822-4300-9F56-263B 2971 3744} //32
			var date = DateTime.Now.ToString("yyyyMMdd");
			var group = CalcGroup(guid);
			var ext = MimeResolver.MimeToFileExtension(media.Mime);

			var file_name = $"{id}{media.Width}-{media.Height}-{media.Length}-{media.Name.LetterOrDigits().Limit(50)}{ext}";
			var basePath = PathResolver.Resolve( RootPath, date, group);
			System.IO.Directory.CreateDirectory(basePath);
			var path = System.IO.Path.Combine(basePath, file_name);


			if (Content is IFileContent)
			{
				System.IO.File.Copy(((IFileContent)Content).FilePath, path);
			}
			else
				using (var fs = new System.IO.FileStream(path, System.IO.FileMode.CreateNew, System.IO.FileAccess.Write, System.IO.FileShare.Read))
				{
					if (Content is IStreamContent sc)
						await sc.Stream.CopyToAsync(fs);
					else if (Content is IByteArrayContent bac)
					{
						var buf = bac.Data;
						await fs.WriteAsync(buf, 0, buf.Length);
					}
					else
					{
						var buf = await Content.GetByteArrayAsync();
						await fs.WriteAsync(buf, 0, buf.Length);
					}

				}
			return $"{date}{id}";
		}
	}
}
