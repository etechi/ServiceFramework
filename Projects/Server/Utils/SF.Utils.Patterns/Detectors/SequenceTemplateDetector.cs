using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
namespace SF.Utils.Patterns
{
	/// <summary>
	/// 返回结果，一级列表项为所有序列独立项目，二级列表项为该项目所在的源序列及索引
	/// </summary>
	public class SequenceTemplateDetector : List<List<KeyValuePair<int, int>>>
	{
		int _sequnece_count;
		int _window_size;
		Func<int, int, int, int, bool> _comparer;
		bool _success;

		public bool Success { get { return _success; } }
		bool Compare(int src_sequence, int src_index, int dst_sequence, int dst_index)
		{
			return _comparer(src_sequence, src_index, dst_sequence, dst_index);
		}
		public SequenceTemplateDetector(
			int[] lengths, 
			Func<int, int, int, int, bool> comparer, 
			int window_size
			)
		{
			_window_size = window_size;
			_sequnece_count = lengths.Length;
			_comparer = comparer;
			var max_length = lengths.Max();
			var min_length = lengths.Min();
			if (max_length - min_length > max_length/4)
				return;

			AddRange(
				Enumerable.Range(0, lengths[0])
				.Select(i => new List<KeyValuePair<int, int>>(lengths.Length) { new KeyValuePair<int, int>(0, i) })
				);

			for (var i = 1; i < lengths.Length; i++)
				AddSequence(i, lengths[i]);
			_success = Count < max_length + 4;
		}
		bool Match(int index, int dst_sequence, int dst_index)
		{
			var p = this[index][0];
			return _comparer(p.Key, p.Value, dst_sequence, dst_index);
		}
		void AddGroupElement(int group_index, int dest_sequence_index, int element_index)
		{
			this[group_index].Add(new KeyValuePair<int, int>(dest_sequence_index, element_index));
		}
		void AddNewGroup(int group_index, int dest_sequence_index, int element_index)
		{
			var g = new List<KeyValuePair<int, int>>(_sequnece_count) { new KeyValuePair<int, int>(dest_sequence_index, element_index) };
			Insert(group_index, g);
		}
		void AddSequence(int dest_index, int dest_elements_length)
		{
			var pci = 0;

			for (var i = 0; i < dest_elements_length; i++)
			{
				var tci = pci;
				var end = Count;
				var count = _window_size;
				for (; count>0 && tci < end; tci++)
				{
					if (Match(tci, dest_index, i))
						break;

					if(this[tci].Count==dest_index)
						count--;
				}
				//#找到
				if (tci < end)
				{
					AddGroupElement(pci, dest_index, i);
					pci++;
				}
				else
					AddNewGroup(pci, dest_index, i);
			}
		}
	}
}
