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
	public class TimerQueue
	{
		public class Timer
		{
			internal Action Callback;
			internal Timer Next;
			internal Timer Prev;
			internal int Left;
			public bool IsInTimerQueue => Next != this;
			public Timer()
			{
				Next = Prev = this;
			}
		}
		class TimerGroup
		{
			public Timer[] Items { get; }
			public TimerGroup(int GroupCount)
			{
				Items = Enumerable
					.Range(0, GroupCount)
					.Select(i => new Timer())
					.ToArray();
			}
			public int NextItemIndex;
		}
		public int MaxLevel { get; }
		public int GroupItemCount { get; }
		public int MaxPeriod { get; }
		TimerGroup[] Groups { get; }
		public TimerQueue(int MaxLevel, int GroupItemCount)
		{
			this.MaxLevel = MaxLevel;
			this.GroupItemCount = GroupItemCount;
			this.MaxPeriod = Enumerable.Repeat(GroupItemCount, MaxLevel)
						.Aggregate((x, y) => x * y);

			Groups = Enumerable.Range(0, MaxLevel).Select(i => new TimerGroup(GroupItemCount)).ToArray();
		}

		public void Trace()
		{
			var lev = 0;
			foreach(var g in Groups)
			{
				Console.WriteLine($"TimeQueueGroup:{lev++} {g.NextItemIndex}");
				for (var i = g.NextItemIndex; i < GroupItemCount; i++)
				{
					var idx = i % GroupItemCount;
					var list = g.Items[idx];
					var first = true;
					for(var n= list.Next;n != list;n=n.Next)
					{
						if (first)
						{
							Console.Write($"\t{idx}:");
							first = false;
						}
						Console.Write($" {n}");
					}
					if (!first)
						Console.WriteLine();
				}
			}
		}

		void AddItem(TimerGroup grp, int idx, Timer item)
		{
			var items = grp.Items;
			var head = items[idx];
			var tail = head.Prev;
			item.Next = head;
			item.Prev = tail;
			tail.Next = item;
			head.Prev = item;
		}
		IEnumerable<Timer> GetEnumerable(Timer item)
		{
			if (item == null)
				yield break;
			var end = item;
			var i = item;
			do
			{
				var next = i.Next;
				i.Next = i.Prev = i;
				yield return i;
				i = next;
			}
			while (i != end);
		}
		Timer GetList(Timer head)
		{
			var list = head.Next == head ? null : head.Next;
			if (list != null)
			{
				head.Prev.Next = list;
				list.Prev = head.Prev;
				head.Next = head.Prev = head;
			}
			return list;
		}
		Timer NextTick(int level, int levelCount)
		{
			var g = Groups[level];

			var list = GetList(g.Items[g.NextItemIndex]);

			g.NextItemIndex++;
			if (g.NextItemIndex == GroupItemCount)
			{
				g.NextItemIndex = 0;
				if (level == MaxLevel - 1)
					return list;
			}
			else
				return list;


			var nextLevItems = NextTick(level + 1, levelCount * GroupItemCount);
			foreach (var item in GetEnumerable(nextLevItems))
			{
				var left = item.Left;
				var idx = left / levelCount;
				item.Left = left % levelCount;
				AddItem(g, idx, item);
			}
			return list;
		}
		public IEnumerable<Timer> NextTick()
		{
			var item = NextTick(0, 1);
			return GetEnumerable(item);
		}
		public bool Remove(Timer Item)
		{
			var next = Item.Next;
			var prev = Item.Prev;
			if (next == Item || prev == Item)
				return false;
			next.Prev = prev;
			prev.Next = next;
			Item.Next = Item.Prev = Item;
			return true;
		}
		public Timer Add(int Target, Timer item)
		{
			if (Target < 0 ||
				Target >= MaxPeriod ||
				item.Next != item ||
				item.Prev != item)
				throw new ArgumentException();

			var left = Target;
			var idx = left;

			var i = 0;
			var levelCount = 1;
			for (; i < MaxLevel; i++)
			{
				var g = Groups[i];
				if (idx < GroupItemCount)
				{
					item.Left = left - idx * levelCount;
					AddItem(g, (g.NextItemIndex + idx) % GroupItemCount, item);
					break;
				}
				var curLevelLeft = GroupItemCount - g.NextItemIndex;
				idx = (idx - curLevelLeft) / GroupItemCount;
				left -= curLevelLeft * levelCount;
				levelCount *= GroupItemCount;
			}
			if (i == MaxLevel)
				throw new InvalidOperationException();
			return item;
		}
		public IEnumerable<Timer> Reset()
		{
			foreach (var g in Groups)
			{
				g.NextItemIndex = 0;
				foreach (var head in g.Items)
				{
					var list = GetList(head);
					if (list != null)
						foreach (var i in GetEnumerable(list))
							yield return i;
				}
			}
		}
	}

}
