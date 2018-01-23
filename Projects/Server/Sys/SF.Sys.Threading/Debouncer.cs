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
using System.Threading;
using System.Threading.Tasks;
namespace SF.Sys.Threading
{
	
	public static class Debouncer
	{
		class Item
		{
			public DateTime StartTime;
			public DateTime ExecTime;
			public DateTime DeadLine;
			public Action<bool> Callback;
		}
		static class Impl<K> where K:IEquatable<K>
		{	
			static Dictionary<K, Item> Dict { get; }
				= new Dictionary<K, Item>();
			public static void Exec(K key, Action<bool> Callback, int MinDelay, int MaxDelay)
			{
				Item item = null;
				var newItem = false;
				lock (Dict)
				{
					if (!Dict.TryGetValue(key, out item))
					{
						Dict[key] = item = new Item();
						newItem = true;
					}
				}
				Action<bool> replacedCallback = null;
				lock (item)
				{
					var now = DateTime.Now;
					if (newItem)
					{
						item.StartTime = now;
						item.DeadLine = now.AddSeconds(MaxDelay);
					}
					else
					{
						replacedCallback = item.Callback;
						//检查是否延时
						var deadLine = item.StartTime.AddSeconds(MaxDelay);
						if (deadLine < item.DeadLine) item.DeadLine = deadLine;
					}

					var execTime = now.AddSeconds(MinDelay);
					item.ExecTime = execTime > item.DeadLine ? item.DeadLine : execTime;
					item.Callback = Callback;
				}

				if (replacedCallback != null)
				{
					try
					{
						replacedCallback(true);
					}
					catch { }
				}
				if (!newItem)
					return;

				Task.Run(() =>
				{	
					Action<bool> cb = null;
					for (; ; )
					{
						var time = DateTime.Now;
						TimeSpan delay;
						lock (item)
						{
							if (item.ExecTime <= time)
							{
								cb = item.Callback;
								lock (Dict)
								{
									Dict.Remove(key);
								}
								break;
							}
							delay = item.ExecTime.Subtract(time);
						}
						Task.Delay(delay);
					}
					try
					{
						cb(false);
					}
					catch { }
				});
			}
		}

		/// <summary>
		/// 开始延时执行
		/// </summary>
		/// <typeparam name="K"></typeparam>
		/// <param name="key">键值</param>
		/// <param name="Callback">延时回调委托,若被取消参数未true,正常执行为true</param>
		/// <param name="MinDelay">最小延时</param>
		/// <param name="MaxDelay">最大延时</param>
		public static void Start<K>(K key,Action<bool> Callback,int MinDelay= 1000, int MaxDelay = 1000 * 20)
			where K:IEquatable<K>
		{
			Impl<K>.Exec(key, Callback, MinDelay, MaxDelay);
		}
    }
}
