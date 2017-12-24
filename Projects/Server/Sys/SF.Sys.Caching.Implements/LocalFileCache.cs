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

using SF.Sys.Hosting;
using SF.Sys.Threading;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SF.Sys.Caching
{
	public class LocalFileCacheSetting
	{
		/// <summary>
		/// 缓存根目录
		/// </summary>
		[Required]
		public string RootPath { get; set; }

		/// <summary>
		/// 路径解析器
		/// </summary>
		[Required]
		public IFilePathResolver PathResolver { get; set; }

	}
	/// <summary>
	/// 本地文件缓存
	/// </summary>
	public class LocalFileCache : IFileCache
	{
		LocalFileCacheSetting Setting { get; }
		public LocalFileCache(LocalFileCacheSetting Setting)
		{
			this.Setting = Setting;
		}
		static ObjectSyncQueue<string> SyncQueue { get; } = new ObjectSyncQueue<string>();

		public async Task<string> Cache(
			string FileName, 
			FileContentGenerator ContentGenerator, 
			string FilePath = null
			)
		{
			if (FilePath == null)
				FilePath = (((uint)FileName.GetConsistencyHashCode()) % 1024).ToString();

			var file_path =Setting.PathResolver.Resolve(Setting.RootPath, FilePath);
			var file = Directory.Exists(file_path) ? Directory.GetFiles(file_path, FileName + ".*").FirstOrDefault() : null;
			if (file != null)
				return file;

			return await SyncQueue.Queue(
				file_path + "\\" + FileName,
				async () =>
				{
					file = Directory.Exists(file_path) ? Directory.GetFiles(file_path, FileName + ".*").FirstOrDefault() : null;
					if (file != null)
						return file;
					Directory.CreateDirectory(file_path);
					var ctn = await ContentGenerator();
					if (ctn==null || ctn.Content == null)
						return null;

					file = Path.Combine(file_path, FileName + (ctn.FileExtension[0] == '.' ? ctn.FileExtension : "." + ctn.FileExtension));
					using (var fs = new FileStream(file, FileMode.CreateNew, FileAccess.Write, FileShare.Read))
						await fs.WriteAsync(ctn.Content, 0, ctn.Content.Length);

					return file;
				});
		}
	}
}
