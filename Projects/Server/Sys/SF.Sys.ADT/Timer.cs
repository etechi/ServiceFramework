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
using System.Collections;
using System.Linq;

namespace SF.Sys.ADT
{
	public class Timer<TValue> : Timer<DateTime, TValue> { }
	public class Timer<TKey,TValue>
	{
		Heap<TKey, TValue> Heap { get; } = new Heap<TKey, TValue>();
		Dictionary<TKey, Item> Dict { get; } = new Dictionary<TKey, Item>();

		public class Item : Heap<TKey,TValue>.Item,IEnumerable<Item>
		{
			public Item(TKey Key, TValue Value) : base(Key, Value)
			{ }
			internal Item Next { get; set; }	
			internal Item Prev { get; set; }

			public IEnumerator<Item> GetEnumerator()
			{
				var end = this;
				var i = this;
				do
				{
					yield return i;
					i = i.Next;
				}
				while (i != end);
			}

			internal void Validate()
			{
				if ((Next == null) != (Prev == null))
					throw new InvalidOperationException("数据错误");
				if(!IsInHeap && (Next==this || Prev==this))
					throw new InvalidOperationException("数据错误");
			}

			IEnumerator IEnumerable.GetEnumerator()
				=>GetEnumerator();
		}
		public int KeyCount => Heap.Count;
		public Item Enqueue(TKey Target,TValue Value)
		{
			var item = new Item(Target, Value);
			if (!Dict.TryGetValue(Target, out var list))
			{
				item.Next = item.Prev = item;
				Dict[Target] = item;
				Heap.Enqueue(item);
			}
			else
			{
				item.Next = list;
				item.Prev = list.Prev;
				item.Prev.Next = item;
				list.Prev = item;
			}
			return item;
		}

		public Item Peek()
		{
			if (Heap.Count == 0)
				throw new InvalidOperationException("没有项目");
			return (Item)Heap.Peek();
		}

		public KeyValuePair<TKey,IEnumerable<Item>> Dequeue()
		{
			if (Heap.Count == 0)
				throw new InvalidOperationException("没有项目");
			var item = (Item)Heap.Dequeue();
			if (!Dict.Remove(item.Key))
				throw new InvalidOperationException("定时器堆栈项目不在字典中");

			return new KeyValuePair<TKey, IEnumerable<Item>>(
				item.Key,
				item
				);
		}


		public void Remove(Item item)
		{
			item.Validate();

			//如果不是最后一个并且不是列表头，可以简单处理
			if (item.Next != item && !item.IsInHeap)
			{
				item.Prev.Next = item.Next;
				item.Next.Prev = item.Prev;
				return;
			}
			var key = item.Key;

			if (!Dict.TryGetValue(key, out var list))
				throw new InvalidOperationException($"操作异常，定时器中没有相应的项目列表");

			//检查是否是列表头
			if (list != item)
				throw new InvalidOperationException($"操作异常，定时器中的项目列表首项和指定项目不一致");

			//如果只有一项
			if (item.Next == item)
			{
				if (!item.IsInHeap)
					throw new InvalidOperationException($"列表最后一个项目不在定时器堆中");
				Dict.Remove(key);
				//从堆中移除
				Heap.Remove(item);
			}
			//如果移除列表头，列表需要指向下一项
			else
			{
				var next = item.Next;
				var prev = item.Prev;

				prev.Next = item.Next;
				next.Prev = item.Prev;

				//更新首项为下一项
				Dict[key] = next;

				//将当前项堆中移除后,加入下一项
				Heap.Remove(item);
				Heap.Enqueue(next);
			}
		}
	}
}
