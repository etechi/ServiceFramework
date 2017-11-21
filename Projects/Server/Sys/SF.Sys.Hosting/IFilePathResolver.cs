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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Sys.Hosting
{
	public class FilePathDefination
	{
		/// <title>数据目录</title>
		/// <summary>
		/// 前缀:data://, 默认为root://data, 用于存放系统数据
		/// </summary>
		public string DataPath { get; set; }
		/// <title>内容目录</title>
		/// <summary>
		/// 前缀:content:// 默认为root://content, 用于存放公开内容
		/// </summary>
		public string ContentPath { get; set; }
		/// <title>临时目录</title>
		/// <summary>
		///默认为root://temp, 用于存放临时文件
		/// </summary>
		public string TempPath { get; set; }
		/// <title>配置目录</title>
		/// <summary>
		/// 默认为root://config, 用于存放配置文件
		/// </summary>
		public string ConfigPath { get; set; }
	}
	public interface IDefaultFilePathStructure
	{
		string RootPath { get; }
		string BinaryPath { get; }
		FilePathDefination FilePathDefination { get; }
	}
	/// <summary>
	/// 文件路径解析器
	/// </summary>
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
