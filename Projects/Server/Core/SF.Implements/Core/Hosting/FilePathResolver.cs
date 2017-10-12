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
