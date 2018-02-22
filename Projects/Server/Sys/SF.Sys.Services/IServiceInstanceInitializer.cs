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
using System.Text;
using System.Threading.Tasks;

namespace SF.Sys.Services
{
	public enum CommandType
	{

	}
	public class Command
	{
		public CommandType Type { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string Icon { get; set; }
		public string Arguments { get; set; }
	}
	public class ServiceInstanceConfig
	{
		public string Name { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public string Ident { get; set; }
		public bool Enabled { get; set; }
	}
	/// <summary>
	/// 服务实例初始化器
	/// </summary>
	public interface IServiceInstanceInitializer
	{
		/// <summary>
		/// 服务配置
		/// </summary>
		ServiceInstanceConfig Config { get; }
		/// <summary>
		/// 添加初始化动作，在初始化时调用
		/// </summary>
		/// <param name="Action"></param>
		void AddAction(Func<IServiceProvider, long, Task> Action);

		/// <summary>
		/// 初始化服务
		/// </summary>
		/// <param name="ServiceProvider"></param>
		/// <param name="ParentId"></param>
		/// <returns></returns>
		Task<long> Ensure(IServiceProvider ServiceProvider, long? ParentId);
	}
	public interface IServiceInstanceInitializer<T> : IServiceInstanceInitializer
	{

	}
}
