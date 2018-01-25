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


namespace SF.Sys.AtLeastOnceTasks
{
	/// <summary>
	/// 任务状态
	/// </summary>
	public enum AtLeastOnceTaskState
	{
		/// <summary>
		/// 已暂停
		/// </summary>
		Paused,
		/// <summary>
		/// 等待执行
		/// </summary>
		Waiting,
		/// <summary>
		/// 正在执行
		/// </summary>
		Running,
		/// <summary>
		/// 执行完成
		/// </summary>
		Completed,
		/// <summary>
		/// 执行错误
		/// </summary>
		Failed
	}
}
