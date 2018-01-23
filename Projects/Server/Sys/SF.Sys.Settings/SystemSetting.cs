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

namespace SF.Sys.Settings
{
	/// <summary>
	/// 系统设置
	/// </summary>
	public class SystemSetting
	{
		/// <summary>
		/// 系统名称
		/// </summary>
		public string SystemName { get; set; } = System.IO.Path.GetDirectoryName( AppDomain.CurrentDomain.BaseDirectory);

		/// <summary>
		/// 系统版本
		/// </summary>
		public string Version { get; set; } = "1.0.0";

		///<title>外部流水后缀</title>
		/// <summary>
		/// 用于系统重置或不同部署环境下,保持外部流水号唯一.
		/// </summary>
		public string ExtIdentPostfix { get; set; } = "00";
	}
}
