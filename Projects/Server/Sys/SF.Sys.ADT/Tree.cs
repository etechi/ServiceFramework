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

using SF.Sys.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SF.Sys.ADT
{
	public static class Tree
	{
		//public static IObservable<T> AsObservable<T>(
		//	T Node,
		//	Func<T,IObservable<T>> GetChildren)
		//{
		//	var cs = GetChildren(Node);
		//	if (cs == null)
		//		return Observable.FromAsync(() => Task.FromResult(Node));
		//	else
		//		return (from o in cs
		//				from c in AsObservable(o, GetChildren)
		//				select c).StartWith(Node);
		//}
		static async Task LoadAllChildren<T>(T Node, Func<T, Task<T[]>> GetChildren,List<T> Result)
		{
			
			var cs = await GetChildren(Node);
			if (cs != null)
				foreach (var c in cs)
				{
					Result.Add(c);
					await LoadAllChildren(c, GetChildren, Result);
				}
		}
		public static async Task<T[]> LoadAllChildren<T>(T Node,Func<T, Task<T[]>> GetChildren)
		{
			var re = new List<T>();
			await LoadAllChildren(Node, GetChildren, re);
			return re.ToArray();
		}
		public static IEnumerable<T> AsEnumerable<T>(
			T Node,
			Func<T,IEnumerable<T>> GetChildren
			)
		{
			yield return Node;
			var cs = GetChildren(Node);
			if (cs != null)
				foreach (var n in cs)
					foreach (var c in AsEnumerable(n, GetChildren))
						yield return c;
		}
		public static IEnumerable<T> AsEnumerable<T>(
			IEnumerable<T> Nodes,
			Func<T, IEnumerable<T>> GetChildren
			)
			=> Nodes.SelectMany(n => AsEnumerable(n, GetChildren));
		public static IEnumerable<T> AsLastEnumerable<T>(
			T Node,
			Func<T, IEnumerable<T>> GetChildren
			)
		{
			var cs = GetChildren(Node);
			if (cs != null)
				foreach (var n in cs)
					foreach (var c in AsEnumerable(n, GetChildren))
						yield return c;
			yield return Node;
		}
		public static IEnumerable<T> AsLastEnumerable<T>(
			IEnumerable<T> Nodes,
			Func<T, IEnumerable<T>> GetChildren
			)
			=> Nodes.SelectMany(n => AsLastEnumerable(n, GetChildren));
		public class Node<T> : List<Node<T>>
		{
			public T Value { get; set; }
		}

		public static IEnumerable<Node<T>> Build<T>(
			this IEnumerable<T> nodes,
			Func<T, T> GetParent
			)
		{
			return nodes.Build<T,Node<T>,T>(
				i => new Node<T> { Value = i },
				i => i,
				i => GetParent(i),
				(i, c) => i.Add(c)
				);
		}
		public static IEnumerable<T> Build<I, T, K>(
			this IEnumerable<I> nodes,
			Func<I, T> NewTreeItem,
			Func<I, K> GetId,
			Func<I, K> GetParentId,
			Action<T, T> AddChildItem
			)
		{
			var dic = nodes.ToDictionary(GetId, i => (TreeItem:NewTreeItem(i), Item:i));
			var roots = new List<T>();
			foreach (var (ti, ii) in dic.Values)
			{
				var pi = GetParentId(ii);
				if (EqualityComparer<K>.Default.Equals(pi, default(K)))
					roots.Add(ti);
				else
				{
					if (!dic.TryGetValue(pi, out var pnt))
						throw new InvalidOperationException($"在节点列表中找不到节点{GetId(ii)}的父节点{pi}");
					var (pti, pii) = pnt;
					AddChildItem(pti, ti);
				}
			}
			return roots;
		}
		public static IEnumerable<I> Build<I, K>(
		   this IEnumerable<I> nodes,
		   Func<I, K> GetId,
		   Func<I, K> GetParentId,
		   Action<I, I> AddChildItem
		   )
		   where K : IEquatable<K>
			=> Build(nodes, i => i, GetId, GetParentId, AddChildItem);



		public static IEnumerable<T> BuildByPath<I, T>(
			this IEnumerable<I> nodes,
			Func<I, T> NewTreeItem,
			Func<I, string[]> GetPath,
			Action<T, T> AddChildItem
			)
		{
			var dic = nodes.Select(i =>
			{
				var path = GetPath(i);
				return new
				{
					item = i,
					parent = path.Take(path.Length - 1).Join("\n"),
					id = path.Join("\n")
				};
			}).ToDictionary(p => p.item);
			return nodes.Build<I, T, string>(
				i => NewTreeItem(i),
				i => dic[i].id,
				i => dic[i].parent,
				AddChildItem
				);
		}

		public static IEnumerable<T> BuildByParentPath<I, T>(
			IEnumerable<I> nodes,
			Func<I[], T> NewTreeItem,
			Func<I, string[]> GetPath,
			Action<T, T> AddChildItem
			)
		{
			var dic = nodes.Select(i =>
			{
				var path = GetPath(i);
				return new
				{
					item = i,
					id = path.Join("\n")
				};
			})
			.GroupBy(p => p.id)
			.ToDictionary(g => g.Key, g => g.ToArray());

			return dic.Build(
				p => NewTreeItem(p.Value.Select(ii => ii.item).ToArray()),
				p => p.Key,
				p => p.Key.LastSplit2('\n').Item1,
				AddChildItem
				);

		}
		public static async Task ValidateTreeParent<TKey>(
			string Title,
			TKey Id,
			TKey NewParentId,
			Func<TKey, Task<TKey>> GetParentId
			) where TKey : IEquatable<TKey>
		{
			//检查父节点不能是自身，或自身的子节点
			if (Id.Equals(default(TKey)) || NewParentId.Equals(default(TKey)))
				return;
			if (Id.Equals(NewParentId))
				throw new PublicArgumentException($"父{Title}不能是当前{Title}自己");

			for (; ; )
			{
				var npid = await GetParentId(NewParentId);
				if (npid.Equals(default(TKey)))
					break;
				if (npid.Equals(Id))
					throw new PublicArgumentException($"父{Title}不能是当{Title}的子{Title}");
				NewParentId = npid;
			}
		}
		public static IEnumerable<X> Merge<X, K>(
			IEnumerable<X> xs,
			IEnumerable<X> ys,
			Func<X, K> KeySelector,
			Func<X, IEnumerable<X>> ChildrenSelector,
			Func<X, X, IEnumerable<X>, X> Merger
			) where K : IEquatable<K>
			=> Merge(xs, KeySelector, ChildrenSelector, ys, KeySelector, ChildrenSelector, Merger);

		public static IEnumerable<Z> Merge<X,Y,Z,K>(
			IEnumerable<X> xs,
			Func<X,K> XKeySelector,
			Func<X,IEnumerable<X>> XChildrenSelector,
			IEnumerable<Y> ys,
			Func<Y, K> YKeySelector,
			Func<Y, IEnumerable<Y>> YChildrenSelector,
			Func<X,Y,IEnumerable<Z>,Z> Merger
			) where K:IEquatable<K>
		{
			return xs.Merge(
				XKeySelector,
				ys,
				YKeySelector,
				(x, y) =>
				{
					var xcs = XChildrenSelector(x);
					var ycs = YChildrenSelector(y);
					var cs = Merge(
						xcs,
						XKeySelector,
						XChildrenSelector,
						ycs,
						YKeySelector,
						YChildrenSelector,
						Merger
						);
					return Merger(x, y, cs);
				}
			);
		}
	}
}

