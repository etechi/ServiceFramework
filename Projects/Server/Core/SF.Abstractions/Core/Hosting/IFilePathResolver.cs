using SF.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Core.Hosting
{
	public class FilePathDefination
	{
		[Comment("数据目录", "前缀:data://, 默认为root://data, 用于存放系统数据")]
		public string DataPath { get; set; }
		[Comment("内容目录", "前缀:content:// 默认为root://content, 用于存放公开内容")]
		public string ContentPath { get; set; }
		[Comment("临时目录", "默认为root://temp, 用于存放临时文件")]
		public string TempPath { get; set; }
		[Comment("配置目录", "默认为root://config, 用于存放配置文件")]
		public string ConfigPath { get; set; }
	}
	public interface IDefaultFilePathStructure
	{
		string RootPath { get; }
		string BinaryPath { get; }
		FilePathDefination FilePathDefination { get; }
	}
	[Comment("文件路径解析器")]
	public interface IFilePathResolver {
		string RootPath { get; }
		string DataPath { get; }
		string TempPath { get; }
		string ContentPath { get; }
		string BinaryPath { get; }
		string ConfigPath { get; }
		string Resolve(string Path,params string[] Extra);
	}

}
