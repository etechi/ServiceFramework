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
	public class Heap<TKey, TValue> 
	{
		public class Item
		{
			internal int Index { get; set; } = -1;
			public bool IsInHeap => Index != -1;
			public Item(TKey Key, TValue Value)
			{
				this.Key = Key;
				this.Value = Value;
			}
			public TKey Key { get; }
			public TValue Value { get;  }
		}

		static Comparer<TKey> Comparer { get; } = Comparer<TKey>.Default;
		List<Item> ItemList { get; } = new List<Item>();

		public Item Enqueue(TKey Key, TValue Value)
			=> Enqueue(new Item(Key, Value));

		public Item Enqueue(Item item)
		{
			if (item.Index != -1)
				throw new InvalidOperationException("项目正在使用");

			item.Index = ItemList.Count;
			ItemList.Add(item);
			Update(item);

			return item;
		}

		public Item Dequeue()
		{
			if (ItemList.Count == 0)
				throw new InvalidOperationException();
			var i = ItemList[0];
			Remove(i);
			return i;
		}

		public Item Peek()
		{
			if (ItemList.Count == 0)
				throw new ArgumentException();
			var item = ItemList[0];
			return item;
		}
		void Update(Item item)
		{
			int k;
			int ti;
			var count = ItemList.Count;

			//get index of current item
			var pos = item.Index;
			if (pos < 0 || pos >= count)
				throw new InvalidOperationException("无效的索引:" + pos);

			//if the current item has children
			ti = (pos << 1) + 1;
			if (ti < count)
			{
				k = pos;
				for (; ti < count;)
				{
					if (ti < count - 1 && Comparer.Compare(ItemList[ti].Key, ItemList[ti + 1].Key) > 0)
						ti++;
					if (Comparer.Compare(item.Key, ItemList[ti].Key) < 0)
						break;
					(ItemList[k] = ItemList[ti]).Index = k;
					k = ti;
					ti = (k << 1) + 1;
				}
				ItemList[k] = item;
				item.Index = k;
			}

			//if the current item is not the root item
			if (pos > 0)
			{
				item = ItemList[pos];
				k = pos;
				while (k > 0)
				{
					ti = (k - 1) >> 1;
					var tii = ItemList[ti];
					if (Comparer.Compare(tii.Key, item.Key) <= 0)
						break;
					ItemList[k] = tii;
					tii.Index = k;
					k = ti;
				}
				item.Index = k;
				ItemList[k] = item;
			}
		}
		public void Remove(Item item)
		{
			int pos;
			Item pt;
			if (item.Index < 0 || item.Index >= ItemList.Count)
				throw new InvalidOperationException("无效的索引:" + item.Index);

			pos = item.Index;
			item.Index = -1;
			pt = ItemList[ItemList.Count - 1];
			ItemList.RemoveAt(ItemList.Count - 1);
			if (item == pt)
				return;
			pt.Index = pos;
			if (ItemList.Count > pos)
			{
				ItemList[pos] = pt;
				Update(pt);
			}
			else
				ItemList.Add(pt);
		}
		public Item RemoveAny()
		{
			if (ItemList.Count == 0)
				throw new InvalidOperationException();
			Item i = ItemList[ItemList.Count - 1];
			ItemList.RemoveAt(ItemList.Count - 1);
			i.Index = -1;
			return i;
		}
		public void Clear()
		{
			ItemList.Clear();
		}

		
		public int Count
		{
			get { return ItemList.Count; }
		}

		public IEnumerator<Item> Items => ItemList.GetEnumerator() ;
		public IEnumerable<TKey> Keys => ItemList.Select(i => i.Key);
		public IEnumerable<TValue> Values => ItemList.Select(i => i.Value);
	};
}
