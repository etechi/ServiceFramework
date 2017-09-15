using SF.Metadata;
using System;
using System.IO;

namespace SF.Core.Hosting
{

	[Comment("文件路径解析器")]
	public class FilePathResolver : IFilePathResolver
	{
		public FilePathDefination Setting { get; }

		public string RootPath => 
			DefaultFilePathDefinationSource.RootPath;

		public string BinaryPath =>
			DefaultFilePathDefinationSource.BinaryPath;

		public string DataPath =>
			Resolve(Setting?.DataPath??DefaultFilePathDefinationSource.FilePathDefination?.DataPath)?? 
			Path.Combine(RootPath, "data");

		public string TempPath =>
			Resolve(Setting?.TempPath ?? DefaultFilePathDefinationSource.FilePathDefination?.TempPath) ??
			Path.Combine(RootPath, "temp");

		public string ContentPath =>
			Resolve(Setting?.ContentPath ?? DefaultFilePathDefinationSource.FilePathDefination?.ContentPath) ??
			Path.Combine(RootPath, "content");

		public string ConfigPath =>
			Resolve(Setting?.ConfigPath?? DefaultFilePathDefinationSource.FilePathDefination?.ConfigPath) ??
			Path.Combine(RootPath, "config");

		public IDefaultFilePathStructure DefaultFilePathDefinationSource { get; }
		public FilePathResolver(
			IDefaultFilePathStructure DefaultFilePathDefinationSource,
			FilePathDefination Setting
			)
		{
			this.Setting = Setting;
			this.DefaultFilePathDefinationSource = DefaultFilePathDefinationSource;
		}
		public string Resolve(string path, params string[] Extra)
		{
			if (path == null)
				return null;
			var i = path.IndexOf("://");
			string basePath;
			if (i == -1)
				basePath = RootPath;
			else
			{
				switch (path.Substring(0, i))
				{
					case "root": basePath = RootPath; break;
					case "content": basePath = ContentPath; break;
					case "temp": basePath = TempPath; break;
					case "data": basePath = DataPath; break;
					case "bin": basePath = BinaryPath; break;
					case "config": basePath = ConfigPath; break;
					default:
						throw new NotSupportedException();
				}
				path = path.Substring(i + 3);
			}
			if (Path.DirectorySeparatorChar != '/')
				path = path.Replace('/', Path.DirectorySeparatorChar);
			if(Extra==null || Extra.Length==0)
				return Path.Combine(basePath, path);
			return Path.Combine(basePath, path, Path.Combine(Extra));
		}
	}

}
