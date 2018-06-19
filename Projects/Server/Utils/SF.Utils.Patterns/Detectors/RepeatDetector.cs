using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using SF.Utils.Patterns;
namespace SF.Utils.Patterns
{
	public delegate bool Comparer(int src_list, int src_index, int dst_list, int dst_index);
	
	//找出若干序列中包含的相同子序列
	public class RepeatDetector
	{
		Comparer _comparer;
		int[] _list_lengths;

		KeyValuePair<int, int>[] _head_patterns;    //seq_index head_length
		KeyValuePair<int, int>[] _tail_patterns;    //seq_index tail_length
		KeyValuePair<int, int>[][] _repeat_patterns;    //[[seq_index item_ident]]

		//第n个序列头部长度
		public KeyValuePair<int, int>[] head_patterns { get { return _head_patterns; } }    //seq_index head_length

		//第n个序列尾部长度
		public KeyValuePair<int, int>[] tail_patterns { get { return _tail_patterns; } }    //seq_index tail_length

		//有n个模式的重复序列
		//每个模式来自第key个序列的第value个项目
		public KeyValuePair<int, int>[][] repeat_patterns { get { return _repeat_patterns; } }  //[[seq_index item_ident]]
		
		//等价节点列表
		class Group : List<KeyValuePair<int, int>>
		{
			public Range[] ranges; //等价节点范围
		}
		List<Group> _groups = new List<Group>();
		bool Compare(int list, int index1, int index2)
		{
			return _comparer(list, index1, list, index2);
		}
		bool Compare(int list1, int index1, int list2, int index2)
		{
			return _comparer(list1, index1, list2, index2);
		}

		void AddItem(int seq_index, int item_index)
		{
			var i = 0;
			for (; i < _groups.Count; i++)
			{
				var cur = _groups[i][0];
				if (Compare(cur.Key, cur.Value, seq_index, item_index))
					break;
			}
			Group g;
			if (i < _groups.Count)
				g = _groups[i];
			else
				_groups.Add(g = new Group { ranges = new Range[_list_lengths.Length] });

			g.Add(new KeyValuePair<int, int>(seq_index, item_index));
			if (g.ranges[seq_index].IsEmpty())
				g.ranges[seq_index] = new Range(item_index, 1);
			else
				g.ranges[seq_index] = g.ranges[seq_index].Union(item_index);

		}
		public bool Detect(int[] list_lengths, Comparer comparer)
		{
			_comparer = comparer;
			var seq_count = list_lengths.Length;
			_list_lengths = list_lengths;

			if (!InitGroups())
				return false;



			var ranges = new Range[seq_count]; //每个序列的列表重复项范围
			var queue = new List<int>();

			if (!CalcClutrue(ranges, queue))
				return false;

			var head_patterns = new List<KeyValuePair<int, int>>();
			var tail_patterns = new List<KeyValuePair<int, int>>();
			int max_head_size = 0;
			int max_tail_size = 0;
			for (var i = 0; i < seq_count; i++)
			{
				if (ranges[i].IsEmpty())
					continue;
				if (ranges[i].Begin > max_head_size)
					max_head_size = ranges[i].Begin;
				head_patterns.Add(new KeyValuePair<int, int>(i, ranges[i].Begin));

				var tsize = _list_lengths[i] - ranges[i].End;
				if (tsize > max_tail_size)
					max_tail_size = tsize;
				tail_patterns.Add(new KeyValuePair<int, int>(i, tsize));
			}
			_head_patterns = max_head_size > 0 ? head_patterns.ToArray() : Array.Empty<KeyValuePair<int, int>>();
			_tail_patterns = max_tail_size > 0 ? tail_patterns.ToArray() : Array.Empty<KeyValuePair<int, int>>();
			_repeat_patterns = queue
				.Select(
					i => _groups[i]
						.Select(p => new KeyValuePair<int, int>(p.Key, p.Value))
						.ToArray()
					)
				.ToArray();
			return true;

		}

		bool InitGroups()
		{
			_groups.Clear();
			var list_len_desc_indexs = Enumerable.Range(0, _list_lengths.Length).ToArray();
			Array.Sort(list_len_desc_indexs,(x, y) => _list_lengths[x] - _list_lengths[y]);


			var last_index = _list_lengths.Length - 1;
			
			//最大序列长度
			var max_length = _list_lengths[list_len_desc_indexs[last_index]];

			//最大等价类长度
			var max_group_count = Math.Max(Math.Min(max_length / 5, 3), 1);

			//边界长度
			var edge_count = Math.Min(max_length / 4, 2);

			//将所有序列的所有节点，按照序列长度倒序，从中间向两侧生成等价类列表
			for (var i = max_length - 1; i >= 0; i--)
			{
				//按长度倒序遍历所有序列
				for (var j = last_index; j >= 0; j--)
				{
					var li = list_len_desc_indexs[j];
					var len = _list_lengths[li];

					//如果序列长度小于目标长度，退出
					if (len <= i)
						break;

					var m = len / 2;
					var idx = len - i - 1;
					var position = m + (idx + 1) / 2 * (idx % 2 == 0 ? 1 : -1);
					AddItem(li, position);

					//等价类太多
					if (_groups.Count > max_group_count)
						return false;
				}
				if (i == edge_count)
				{
					max_group_count = _groups.Count + Math.Min(max_length / 4, 2);
				}
			}
			return true;
		}

		//计算传递闭包
		//将所有模式按使用个数从大到小排序
		//将前面覆盖率超过80%的模式作为种子
		//搜索覆盖部分包含的新模式
		//如果有新模式，重复前一步
		//否则闭包中的模式为列表项使用模式

		bool CalcClutrue(Range[] ranges, List<int> queue)
		{
			var seq_count = _list_lengths.Length;
			var total_length = _list_lengths.Sum();
			_groups.Sort((x, y) => y.Count - x.Count);
			var count = 0;
			for (var i = 0; i < _groups.Count; i++)
			{
				count += _groups[i].Count;
				queue.Add(i);
				if (count * 5 >= total_length * 4)
					break;
			}
			//获取剩余模式索引
			var left_groups = new List<int>();
			for (var i = queue.Count; i < _groups.Count; i++)
				left_groups.Add(i);

			//计算闭包
			var cur_length = 0;
			for (;;)
			{
				var new_length = queue.Count;
				//根据新模式的应用范围调整各序列的重复项范围
				for (var k = cur_length; k < new_length; k++)
				{
					var rs = _groups[queue[k]].ranges;
					for (var ri = 0; ri < seq_count; ri++)
						ranges[ri] = ranges[ri].Union(rs[ri]);
				}
				//在更新后的重复项范围内查找剩余模式
				for (var k = 0; k < left_groups.Count; k++)
				{
					var g = _groups[left_groups[k]];
					var t = 0;
					for (; t < seq_count; t++)
						if (ranges[t].Contains(g.ranges[t]))
							break;

					//如果找到当前模式，从剩余模式中删除找到的模式
					if (t < seq_count)
					{
						queue.Add(left_groups[k]);
						var end_index = left_groups.Count - 1;
						if (k < end_index)
							left_groups[k] = left_groups[end_index];
						left_groups.RemoveAt(end_index);
						k--;
					}
				}
				if (queue.Count == new_length)
					break;
				cur_length = new_length;
			}

			//如果闭包计算后模式过多，失败
			return queue.Count <= 4;
		}
		//l m 0 1 2 3 4			
		//1 0 0
		//2 1 1 0
		//3 1 1 0 2
		//4 2 2 1 3 0
		//5 2 2 1 3 0 4

	}

}
