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
using System.Collections.Concurrent;

namespace System.Threading.Tasks
{
	public static class TaskHelpersExtensions
	{
		abstract class TaskResultGetter
		{
			public abstract Task<object> GetResult(Task task);
		}
		class TaskResultGetter<T> : TaskResultGetter
		{
			public override async Task<object> GetResult(Task task)
			{
				return await (Task<T>)task;
			}
		}
		static ConcurrentDictionary<Type, TaskResultGetter> Getters { get; } = new ConcurrentDictionary<Type, TaskResultGetter>();

		public static async Task<object> CastToObject(this Task task)
		{
			var type = task.GetType();
			if(type==typeof(Task))
			{
				await task;
				return null;
			}
			if (!Getters.TryGetValue(type, out var getter))
				getter = Getters.GetOrAdd(
					type,
					(TaskResultGetter)Activator.CreateInstance(
						typeof(TaskResultGetter<>).MakeGenericType(type.GetGenericArguments())
						)
					);
			return await getter.GetResult(task);
		}

		public static async Task<object> CastToObject<T>(this Task<T> task)
		{
			return await task;
		}

		public static void ThrowIfFaulted(this Task task)
		{
			task.GetAwaiter().GetResult();
		}

		public static bool TryGetResult<TResult>(this Task<TResult> task, out TResult result)
		{
			if (task.Status == TaskStatus.RanToCompletion)
			{
				result = task.Result;
				return true;
			}
			result = default(TResult);
			return false;
		}
	}
}
