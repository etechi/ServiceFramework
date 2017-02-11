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
	}
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
			SemaphoreSlim Locker, 
			Func<Task<KeyValuePair<string, byte[]>>> Loader, 
			string FilePath = null
			)
		{
			if (FilePath == null)
				FilePath = (((uint)FileName.GetConsistencyHashCode()) % 1024).ToString();

			var file_path = Path.Combine(Setting.RootPath, FilePath);
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
					var ctn = await Loader();
					if (ctn.Value == null)
						return null;

					file = Path.Combine(file_path, FileName + (ctn.Key[0] == '.' ? ctn.Key : "." + ctn.Key));
					using (var fs = new FileStream(file, FileMode.CreateNew, FileAccess.Write, FileShare.Read))
						await fs.WriteAsync(ctn.Value, 0, ctn.Value.Length);

					return file;
				});
		}
	}
}
