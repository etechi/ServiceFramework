using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Generic
{
	public enum PositionModifyAction
	{
		First,
		Previous,
		Next,
		Last,
		Insert
	}
	public static class IListExtension
	{
		public static bool ModifyPosition<I>(
			this IList<I> List,
			I Model,
			PositionModifyAction Action,
			Predicate<I> IsCurItem,
			Func<I, int> GetPosition,
			Action<I, int> SetPosition
			)
		{
			var curIndex = List.IndexOf(Model);
			switch (Action)
			{
				case PositionModifyAction.Insert:
					if (curIndex == GetPosition(Model))
						return false;
					if (curIndex != -1)
						List.RemoveAt(curIndex);
					var newPos = GetPosition(Model);
					if (newPos < List.Count)
						List.Insert(newPos, Model);
					else
						List.Add(Model);
					break;
				case PositionModifyAction.First:
					if (curIndex != -1)
					{
						if (curIndex == 0)
							return false;
						List.RemoveAt(curIndex);
					}
					List.Insert(0, Model);
					break;
				case PositionModifyAction.Last:
					if (curIndex != -1)
					{
						if (curIndex == List.Count - 1)
							return false;
						List.RemoveAt(curIndex);
					}
					List.Add(Model);
					break;
				case PositionModifyAction.Next:
					if (curIndex != -1)
					{
						if (curIndex == List.Count - 1)
							return false;
						List.RemoveAt(curIndex);
					}
					if (curIndex == -1 || curIndex >= List.Count - 1)
						List.Add(Model);
					else
						List.Insert(curIndex + 1, Model);
					break;
				case PositionModifyAction.Previous:
					if (curIndex != -1)
					{
						if (curIndex == 0)
							return false;
						List.RemoveAt(curIndex);
					}
					if (curIndex == -1)
						List.Insert(0, Model);
					else
						List.Insert(curIndex - 1, Model);
					break;
			}
			for (var i = 0; i < List.Count; i++)
				if (GetPosition(List[i]) != i)
					SetPosition(List[i], i);
			return true;
		}
	}
}
