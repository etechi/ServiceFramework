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
using System.Threading.Tasks;

namespace SF.Sys.NetworkService
{
	public class ClientSetting
	{
		/// <summary>
		/// 当前系统时间
		/// </summary>
		public DateTime Time { get; set; }
		/// <summary>
		/// 应用名称
		/// </summary>
		public string SystemName { get; set; }

		/// <summary>
		/// 服务端版本
		/// </summary>
		public string Version { get; set; }
		
		/// <summary>
		/// 主域名
		/// </summary>
		public string MainDomain { get; set; }

		/// <summary>
		/// API基地址
		/// </summary>
		public string ApiBase { get; set; }

		/// <summary>
		/// H5App基地址
		/// </summary>
		public string H5AppBase { get; set; }
		
		/// <summary>
		/// 资源文件基地址
		/// </summary>
		public string ResBase { get; set; }

		/// <summary>
		/// 是否是HTTPS模式
		/// </summary>
		public bool HttpsMode { get; set; }

		/// <summary>
		/// 图片基地址
		/// </summary>
		public string ImageBase { get; set; }

		/// <summary>
		/// 其他应用设置
		/// </summary>
		public Dictionary<string, object> Options { get; set; }
	}

	public interface IClientSettingProvider
	{
		string Name { get; }
		Task<object> GetOption(string ClientId);
	}
	/// <summary>
	/// 客户端设置服务
	/// </summary>
	[NetworkService]
	public interface IClientSettingService
	{
		/// <summary>
		/// 获取
		/// </summary>
		/// <returns></returns>
		Task<ClientSetting> GetSettings(string ClientId);
	}
}
