using SF.Core.Hosting;
using SF.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SF.Core.Caching
{
	public class LocalFileCacheSetting
	{
		[Comment("缓存根目录")]
		[Required]
		public string RootPath { get; set; }

		[Comment("路径解析器")]
		[Required]
		public IFilePathResolver PathResolver { get; set; }

	}
	[Comment("本地文件缓存")]
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
