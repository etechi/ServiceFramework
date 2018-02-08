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
using System.Threading;

namespace SF.Sys.Threading
{
	public interface ISyncScope
	{
		Task<T> Sync<T>(Func<Task<T>> callback);
	}
	public static class SyncScopeExtension
	{
		public static Task Sync(this ISyncScope SyncScope,Func<Task> callback)
		{
			return SyncScope.Sync(async () =>
			{
				await callback();
				return 0;
			});
		}
	}
	public class SyncScope : ISyncScope
	{
		abstract class Item
		{
			public Item Next;
			public abstract Task Execute();
		}
		class Item<T> : Item
		{

			public Func<Task<T>> Callback;
			public TaskCompletionSource<T> TCS;
			public override async Task Execute()
			{
				try
				{
					var re=await Callback();
					TCS.TrySetResult(re);
				}
				catch (Exception ex)
				{
					TCS.TrySetException(ex);
				}
				Callback = null;
			}

		}
		Item _Head;
		Item _Tail;

		public Task<T> Sync<T>(Func<Task<T>> callback)
		{
			var item = new Item<T> { Callback = callback };
			var tcs=item.TCS = new TaskCompletionSource<T>();
			lock (this)
			{ 
				if (_Head == null)
				{
					tcs = null;
					_Head = item;
				}
				else
					_Tail.Next = item;
				_Tail = item;
			}
			if (tcs != null)
				return tcs.Task;

			var re = callback();
			re.ContinueWith(async t =>
			{
				for (; ; )
				{
					Item next;
					lock (this)
					{
						next = _Head.Next;
						_Head = next;
						if (next == null)
							_Tail = null;
					}
					if (next == null)
						break;
					try
					{
						await next.Execute();
					}
					catch { }
				}

			});
			return re;
		}
	
	}
}
